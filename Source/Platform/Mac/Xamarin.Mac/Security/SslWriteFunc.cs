using Xamarin.Mac.System.Mac;

namespace Security;

internal delegate SslStatus SslWriteFunc(IntPtr connection, IntPtr data, ref nint dataLength);
