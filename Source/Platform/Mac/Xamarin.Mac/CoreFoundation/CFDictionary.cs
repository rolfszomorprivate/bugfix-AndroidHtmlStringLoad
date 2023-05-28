using System.Collections.Generic;
using System.Runtime.InteropServices;
using ObjCRuntime;
using Xamarin.Mac.System.Mac;

namespace CoreFoundation;

internal class CFDictionary : INativeObject, IDisposable
{
	public static IntPtr KeyCallbacks;

	public static IntPtr ValueCallbacks;

	public IntPtr Handle { get; private set; }

	public nint Count => CFDictionaryGetCount(Handle);

	public CFDictionary(IntPtr handle)
		: this(handle, owns: false)
	{
	}

	public CFDictionary(IntPtr handle, bool owns)
	{
		if (!owns)
		{
			CFObject.CFRetain(handle);
		}
		Handle = handle;
	}

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation", EntryPoint = "CFDictionaryGetTypeID")]
	public static extern nint GetTypeID();

	~CFDictionary()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (Handle != IntPtr.Zero)
		{
			CFObject.CFRelease(Handle);
			Handle = IntPtr.Zero;
		}
	}

	static CFDictionary()
	{
		IntPtr handle = Libraries.CoreFoundation.Handle;
		KeyCallbacks = Dlfcn.GetIndirect(handle, "kCFTypeDictionaryKeyCallBacks");
		ValueCallbacks = Dlfcn.GetIndirect(handle, "kCFTypeDictionaryValueCallBacks");
	}

	public static CFDictionary FromObjectAndKey(INativeObject obj, INativeObject key)
	{
		return new CFDictionary(CFDictionaryCreate(IntPtr.Zero, new IntPtr[1] { key.Handle }, new IntPtr[1] { obj.Handle }, 1, KeyCallbacks, ValueCallbacks), owns: true);
	}

	public static CFDictionary FromObjectsAndKeys(INativeObject[] objects, INativeObject[] keys)
	{
		if (objects == null)
		{
			throw new ArgumentNullException("objects");
		}
		if (keys == null)
		{
			throw new ArgumentNullException("keys");
		}
		if (objects.Length != keys.Length)
		{
			throw new ArgumentException("The length of both arrays must be the same");
		}
		IntPtr[] array = new IntPtr[keys.Length];
		IntPtr[] array2 = new IntPtr[keys.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = keys[i].Handle;
			array2[i] = objects[i].Handle;
		}
		return new CFDictionary(CFDictionaryCreate(IntPtr.Zero, array, array2, array.Length, KeyCallbacks, ValueCallbacks), owns: true);
	}

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	private static extern IntPtr CFDictionaryCreate(IntPtr allocator, IntPtr[] keys, IntPtr[] vals, nint len, IntPtr keyCallbacks, IntPtr valCallbacks);

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	private static extern IntPtr CFDictionaryGetValue(IntPtr theDict, IntPtr key);

	public static IntPtr GetValue(IntPtr theDict, IntPtr key)
	{
		return CFDictionaryGetValue(theDict, key);
	}

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	private static extern nint CFDictionaryGetCount(IntPtr theDict);

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	private static extern void CFDictionaryGetKeysAndValues(IntPtr theDict, IntPtr[] keys, IntPtr[] values);

	public void GetKeysAndValues(out IntPtr[] keys, out IntPtr[] values)
	{
		nint count = Count;
		keys = new IntPtr[(long)count];
		values = new IntPtr[(long)count];
		CFDictionaryGetKeysAndValues(Handle, keys, values);
	}

	public static bool GetBooleanValue(IntPtr theDict, IntPtr key)
	{
		IntPtr value = GetValue(theDict, key);
		if (value == IntPtr.Zero)
		{
			return false;
		}
		return CFBoolean.GetValue(value);
	}

	public string GetStringValue(string key)
	{
		using CFString cFString = new CFString(key);
		return CFString.FetchString(CFDictionaryGetValue(Handle, cFString.Handle));
	}

	public int GetInt32Value(string key)
	{
		int value = 0;
		using CFString cFString = new CFString(key);
		if (!CFNumberGetValue(CFDictionaryGetValue(Handle, cFString.Handle), (nint)3, out value))
		{
			throw new KeyNotFoundException($"Key {key} not found");
		}
		return value;
	}

	public long GetInt64Value(string key)
	{
		long value = 0L;
		using CFString cFString = new CFString(key);
		if (!CFNumberGetValue(CFDictionaryGetValue(Handle, cFString.Handle), (nint)4, out value))
		{
			throw new KeyNotFoundException($"Key {key} not found");
		}
		return value;
	}

	public IntPtr GetIntPtrValue(string key)
	{
		using CFString cFString = new CFString(key);
		return CFDictionaryGetValue(Handle, cFString.Handle);
	}

	public CFDictionary GetDictionaryValue(string key)
	{
		using CFString cFString = new CFString(key);
		IntPtr intPtr = CFDictionaryGetValue(Handle, cFString.Handle);
		return (intPtr == IntPtr.Zero) ? null : new CFDictionary(intPtr);
	}

	public bool ContainsKey(string key)
	{
		using CFString cFString = new CFString(key);
		return CFDictionaryContainsKey(Handle, cFString.Handle);
	}

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	internal static extern bool CFNumberGetValue(IntPtr number, nint theType, out int value);

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	internal static extern bool CFNumberGetValue(IntPtr number, nint theType, out long value);

	[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
	private static extern bool CFDictionaryContainsKey(IntPtr theDict, IntPtr key);
}
