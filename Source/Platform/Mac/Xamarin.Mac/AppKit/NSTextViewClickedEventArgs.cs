using CoreGraphics;
using Xamarin.Mac.System.Mac;

namespace AppKit;

public class NSTextViewClickedEventArgs : EventArgs
{
	public NSTextAttachmentCell Cell { get; set; }

	public CGRect CellFrame { get; set; }

	public nuint CharIndex { get; set; }

	public NSTextViewClickedEventArgs(NSTextAttachmentCell cell, CGRect cellFrame, nuint charIndex)
	{
		Cell = cell;
		CellFrame = cellFrame;
		CharIndex = charIndex;
	}
}
