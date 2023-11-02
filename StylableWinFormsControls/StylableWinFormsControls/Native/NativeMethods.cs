using System.Runtime.InteropServices;

namespace StylableWinFormsControls.Native;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Security",
    "CA5392:Use DefaultDllImportSearchPaths attribute for P/Invokes",
    Justification = "All assemblies are commonly used system assemblies.")]
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Style",
    "IDE1006:Naming Styles",
    Justification = "Naming of structures is defined by the native methods that use them.")]
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Roslynator",
    "RCS1135:Declare enum member with zero value (when enum has FlagsAttribute).",
    Justification = "Naming of structures is defined by the native methods that use them.")]
internal class NativeMethods
{
    public const int TRUE_VALUE = 1;
    public const int FALSE_VALUE = 0;
    public const uint CLR_INVALID = 0xFFFFFFFF;
    [DllImport("user32.dll")]
    private static extern int SendMessage(IntPtr wnd, int msg, bool param, int lparam);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern IntPtr GetWindowDC(IntPtr handle);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern IntPtr ReleaseDC(IntPtr handle, IntPtr hDc);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern int GetClassName(IntPtr hwnd, char[] className, int maxCount);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern IntPtr GetWindow(IntPtr hwnd, int uCmd);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern bool IsWindowVisible(IntPtr hwnd);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int GetClientRect(IntPtr hwnd, [In, Out] ref Rectangle rect);
    internal static bool GetClientRectInternal(IntPtr hwnd, ref Rectangle rect)
    {
        return GetClientRect(hwnd, ref rect) == TRUE_VALUE;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern bool InvalidateRect(IntPtr hwnd, ref Rectangle rect, bool bErase);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern bool ValidateRect(IntPtr hwnd, ref Rectangle rect);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern bool GetWindowRect(IntPtr hWnd, [In, Out] ref Rectangle rect);

    [DllImport("user32.dll", EntryPoint = "SendMessageW", SetLastError = true)]
    private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, ref IntPtr lParam);

    [DllImport("user32.dll", EntryPoint = "SendMessageW", SetLastError = true)]
    private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, ref LVGROUP lParam);
    internal static void SendMessageInternal(IntPtr hWnd, int msg, int wParam, ref LVGROUP lParam)
    {
        bool success = SendMessage(hWnd, msg, wParam, ref lParam) == TRUE_VALUE;
        if (!success)
        {
            throw new NativeException($"failed to do native call 'SendMessage' (msg = {NativeConstants.Messages.Reverse(msg)}, wParam = {wParam})", Marshal.GetLastWin32Error());
        }
    }

    [DllImport("user32.dll", EntryPoint = "SendMessageW", SetLastError = true)]
    private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, ref RECT lParam);
    internal static void SendMessageInternal(IntPtr hWnd, int msg, int wParam, ref RECT lParam)
    {
        bool success = SendMessage(hWnd, msg, wParam, ref lParam) == TRUE_VALUE;
        if (!success)
        {
            throw new NativeException($"failed to do native call 'SendMessage' (msg = {NativeConstants.Messages.Reverse(msg)}, wParam = {wParam})", Marshal.GetLastWin32Error());
        }
    }

    [DllImport("user32.dll", EntryPoint = "PostMessageW", SetLastError = true)]
    internal static extern int PostMessage(IntPtr hWnd, int msg, int wParam, ref IntPtr lParam);

    [DllImport("user32.dll")]
    internal static extern IntPtr BeginPaint(IntPtr hwnd, out PAINTSTRUCT lpPaint);

    [DllImport("user32.dll")]
    internal static extern bool EndPaint(IntPtr hWnd, [In] ref PAINTSTRUCT lpPaint);

    [DllImport("gdi32.dll")]
    internal static extern IntPtr CreateSolidBrush(int color);

    /// <summary>
    /// The SetBkColor function sets the current background color to the specified color value,
    /// or to the nearest physical color if the device cannot represent the specified color value.
    /// </summary>
    /// <param name="hdc">A handle to the device context</param>
    /// <param name="color">The new background color.</param>
    /// <returns>
    /// If the function succeeds, the return value specifies the previous background color as a COLORREF value. <br/>
    /// If the function fails, the return value is CLR_INVALID.
    /// </returns>
    /// <remarks>
    /// This function fills the gaps between styled lines drawn using a pen created by the CreatePen function;
    /// it does not fill the gaps between styled lines drawn using a pen created by the ExtCreatePen function.
    /// The SetBkColor function also sets the background colors for TextOut and ExtTextOut. <Br/>
    /// If the background mode is OPAQUE, the background color is used to fill gaps between styled lines, gaps between hatched lines in brushes, and character cells.
    /// The background color is also used when converting bitmaps from color to monochrome and vice versa.
    /// </remarks>
    [DllImport("gdi32.dll")]
    private static extern uint SetBkColor(IntPtr hdc, int color);
    internal static void SetBkColorInternal(IntPtr hdc, int color)
    {
        bool success = SetBkColor(hdc, color) != CLR_INVALID;
        if (!success)
        {
            throw new NativeException($"failed to do native call 'SetBkColor' (color = {color})", Marshal.GetLastWin32Error());
        }
    }

    /// <summary>
    /// The SetBkMode function sets the background mix mode of the specified device context.
    /// </summary>
    /// <param name="bkMode">Can be <see cref="BKM_TRANSPARENT"/> or <see cref="BKM_OPAQUE"/></param>
    /// <returns>
    /// If the function succeeds, the return value specifies the previous background mode.<br/>
    /// If the function fails, the return value is zero.
    /// </returns>
    [DllImport("gdi32.dll")]
    private static extern int SetBkMode(IntPtr hdc, int bkMode);
    internal static void SetBkModeInternal(IntPtr hdc, int bkMode)
    {
        int oldValue = SetBkMode(hdc, bkMode);
        bool success = oldValue is NativeConstants.BKM_OPAQUE or NativeConstants.BKM_TRANSPARENT;
        if (!success)
        {
            throw new NativeException($"failed to do native call 'SetBkMode' (bkMode = {bkMode})", Marshal.GetLastWin32Error());
        }
    }

    /// <summary>
    /// The SetTextColor function sets the text color for the specified device context to the specified color.
    /// </summary>
    /// <param name="hdc"></param>
    /// <param name="color"></param>
    /// <returns>
    /// If the function succeeds, the return value is a color reference for the previous text color as a COLORREF value. <br/>
    /// If the function fails, the return value is CLR_INVALID.
    /// </returns>
    /// <remarks>
    /// The text color is used to draw the face of each character written by the TextOut and ExtTextOut functions. <br/>
    /// The text color is also used in converting bitmaps from color to monochrome and vice versa.
    /// </remarks>
    [DllImport("gdi32.dll")]
    private static extern uint SetTextColor(IntPtr hdc, int color);
    internal static void SetTextColorInternal(IntPtr hdc, int color)
    {
        bool success = SetTextColor(hdc, color) != CLR_INVALID;
        if (!success)
        {
            throw new NativeException($"failed to do native call 'SetTextColor' (color = {color})", Marshal.GetLastWin32Error());
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NMHDR
    {
        internal IntPtr hwndFrom;
        internal IntPtr idFrom;
        internal int code;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        internal int left;
        internal int top;
        internal int right;
        internal int bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NMCUSTOMDRAW
    {
        internal NMHDR hdr;
        internal int dwDrawStage;
        internal IntPtr hdc;
        internal RECT rc;
        internal IntPtr dwItemSpec;
        internal uint uItemState;
        internal IntPtr lItemlParam;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NMLVCUSTOMDRAW
    {
        internal NMCUSTOMDRAW nmcd;
        internal int clrText;
        internal int clrTextBk;
        internal int iSubItem;
        internal int dwItemType;
        internal int clrFace;
        internal int iIconEffect;
        internal int iIconPhase;
        internal int iPartId;
        internal int iStateId;
        internal RECT rcText;
        internal uint uAlign;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct LVGROUP
    {
        internal uint cbSize;
        internal uint mask;
        internal IntPtr pszHeader;
        internal int cchHeader;
        internal IntPtr pszFooter;
        internal int cchFooter;
        internal int iGroupId;
        internal uint stateMask;
        internal uint state;
        internal uint uAlign;
        internal IntPtr pszSubtitle;
        internal uint cchSubtitle;
        internal IntPtr pszTask;
        internal uint cchTask;
        internal IntPtr pszDescriptionTop;
        internal uint cchDescriptionTop;
        internal IntPtr pszDescriptionBottom;
        internal uint cchDescriptionBottom;
        internal int iTitleImage;
        internal int iExtendedImage;
        internal int iFirstItem;
        internal uint cItems;
        internal IntPtr pszSubsetTitle;
        internal uint cchSubsetTitle;
    }

    [Flags]
    internal enum CDRF
    {
        DoDefault = 0x0,
        NewFont = 0x2,
        SkipDefault = 0x4,
        DoErase = 0x8,
        NotifyPostPaint = 0x10,
        NotifyItemDraw = 0x20,
        NotifySubItemDraw = NotifyItemDraw,
        NotifyPostErase = 0x40,
        SkipPostPaint = 0x100,
    }

    [Flags]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Roslynator",
        "RCS1191:Declare enum value as combination of names.",
        Justification = "Wouldn't be semantically accurate.")]
    internal enum CDDS
    {
        PrePaint = 0x1,
        PostPaint = 0x2,
        PreErase = 0x3,
        PostErase = 0x4,
        Item = 0x10000,
        ItemPrePaint = Item | PrePaint,
        ItemPostPaint = Item | PostPaint,
        ItemPreErase = Item | PreErase,
        ItemPostErase = Item | PostErase,
        SubItem = 0x20000
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PAINTSTRUCT
    {
        public IntPtr hdc;
        public bool fErase;
        public RECT rcPaint;
        public bool fRestore;
        public bool fIncUpdate;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] rgbReserved;
    }
}
