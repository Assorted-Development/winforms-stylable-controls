﻿using System.Runtime.InteropServices;

namespace StylableWinFormsControls.Native;

internal class NativeMethods
{
    [DllImport("user32.dll")]
    internal static extern int SendMessage(IntPtr wnd, int msg, bool param, int lparam);
    
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern IntPtr GetWindowDC(IntPtr handle);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern IntPtr ReleaseDC(IntPtr handle, IntPtr hDC);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern int GetClassName(IntPtr hwnd, char[] className, int maxCount);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern IntPtr GetWindow(IntPtr hwnd, int uCmd);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern bool IsWindowVisible(IntPtr hwnd);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern int GetClientRect(IntPtr hwnd, [In, Out] ref Rectangle rect);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern bool InvalidateRect(IntPtr hwnd, ref Rectangle rect, bool bErase);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern bool ValidateRect(IntPtr hwnd, ref Rectangle rect);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern bool GetWindowRect(IntPtr hWnd, [In, Out] ref Rectangle rect);
    
    [DllImport("user32.dll", EntryPoint = "SendMessageW", SetLastError = true)]
    internal static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, ref IntPtr lParam);

    [DllImport("user32.dll", EntryPoint = "SendMessageW", SetLastError = true)]
    internal static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, ref LVGROUP lParam);

    [DllImport("user32.dll", EntryPoint = "SendMessageW", SetLastError = true)]
    internal static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, ref RECT lParam);

    [DllImport("user32.dll", EntryPoint = "PostMessageW", SetLastError = true)]
    internal static extern int PostMessage(IntPtr hWnd, int Msg, int wParam, ref IntPtr lParam);

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
    internal static extern int SetBkColor(IntPtr hdc, int color);

    /// <summary>
    /// The SetBkMode function sets the background mix mode of the specified device context.
    /// </summary>
    /// <param name="bkMode">Can be <see cref="BKM_TRANSPARENT"/> or <see cref="BKM_OPAQUE"/></param>
    /// <returns>
    /// If the function succeeds, the return value specifies the previous background mode.<br/>
    /// If the function fails, the return value is zero.
    /// </returns>
    [DllImport("gdi32.dll")]
    internal static extern int SetBkMode(IntPtr hdc, int bkMode);

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
    internal static extern uint SetTextColor(IntPtr hdc, int color);

    /// <summary>
    /// Background remains untouched.
    /// </summary>
    internal const int BKM_TRANSPARENT = 1;
    /// <summary>
    /// Background is filled with the current background color before the text, hatched brush, or pen is drawn.
    /// </summary>
    internal const int BKM_OPAQUE = 2;

    internal const int LVCDI_ITEM = 0x0;
    internal const int LVCDI_GROUP = 0x1;
    internal const int LVCDI_ITEMSLIST = 0x2;

    internal const int LVM_FIRST = 0x1000;
    internal const int LVM_GETITEMRECT = LVM_FIRST + 14;
    internal const int LVM_GETGROUPRECT = LVM_FIRST + 98;
    internal const int LVM_GETGROUPINFO = LVM_FIRST + 149;

    /// <summary>
    /// Posted when the user releases the left mouse button while the cursor is in the client area of a window.
    /// </summary>
    internal const int WM_LBUTTONUP = 0x202;

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
        // <MarshalAs(UnmanagedType.LPTStr)>
        // internal pszHeader As String
        internal IntPtr pszHeader;
        internal int cchHeader;
        // <MarshalAs(UnmanagedType.LPTStr)>
        // internal pszFooter As String
        internal IntPtr pszFooter;
        internal int cchFooter;
        internal int iGroupId;
        internal uint stateMask;
        internal uint state;
        internal uint uAlign;

        // <MarshalAs(UnmanagedType.LPTStr)>
        // internal pszSubtitle As String
        internal IntPtr pszSubtitle;
        internal uint cchSubtitle;
        // <MarshalAs(UnmanagedType.LPTStr)>
        // internal pszTask As String
        internal IntPtr pszTask;
        internal uint cchTask;
        // <MarshalAs(UnmanagedType.LPTStr)>
        // internal pszDescriptionTop As String
        internal IntPtr pszDescriptionTop;
        internal uint cchDescriptionTop;
        // <MarshalAs(UnmanagedType.LPTStr)>
        // internal pszDescriptionBottom As String
        internal IntPtr pszDescriptionBottom;
        internal uint cchDescriptionBottom;
        internal int iTitleImage;
        internal int iExtendedImage;
        internal int iFirstItem;
        internal uint cItems;
        // <MarshalAs(UnmanagedType.LPTStr)>
        // internal pszSubsetTitle As String
        internal IntPtr pszSubsetTitle;
        internal uint cchSubsetTitle;
    }


    [Flags]
    internal enum CDRF
    {
        CDRF_DODEFAULTField = 0x0,
        CDRF_NEWFONTField = 0x2,
        CDRF_SKIPDEFAULTField = 0x4,
        CDRF_DOERASEField = 0x8,
        CDRF_SKIPPOSTPAINTField = 0x100,
        CDRF_NOTIFYPOSTPAINTField = 0x10,
        CDRF_NOTIFYITEMDRAWField = 0x20,
        CDRF_NOTIFYSUBITEMDRAWField = 0x20,
        CDRF_NOTIFYPOSTERASEField = 0x40
    }

    [Flags]
    internal enum CDDS
    {
        CDDS_PREPAINTField = 0x1,
        CDDS_POSTPAINTField = 0x2,
        CDDS_PREERASEField = 0x3,
        CDDS_POSTERASEField = 0x4,
        CDDS_ITEMField = 0x10000,
        CDDS_ITEMPREPAINTField = CDDS_ITEMField | CDDS_PREPAINTField,
        CDDS_ITEMPOSTPAINTField = CDDS_ITEMField | CDDS_POSTPAINTField,
        CDDS_ITEMPREERASEField = CDDS_ITEMField | CDDS_PREERASEField,
        CDDS_ITEMPOSTERASEField = CDDS_ITEMField | CDDS_POSTERASEField,
        CDDS_SUBITEMField = 0x20000
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

    [StructLayout(LayoutKind.Sequential)]
    public struct DRAWITEMSTRUCT
    {
        public uint CtlType;
        public uint CtlId;
        public uint itemId;
        public uint itemAction;
        public uint itemState;
        public IntPtr hwndItem;
        public IntPtr hdc;
        public RECT rcItem;
        public UIntPtr itemData;
    }

    // Listview group specific flags
    internal const int LVGF_NONE = 0x0;
    internal const int LVGF_HEADER = 0x1;
    internal const int LVGF_FOOTER = 0x2;
    internal const int LVGF_STATE = 0x4;
    internal const int LVGF_ALIGN = 0x8;
    internal const int LVGF_GROUPID = 0x10;

    internal const int LVGF_SUBTITLE = 0x100; // pszSubtitle is valid
    internal const int LVGF_TASK = 0x200; // pszTask is valid
    internal const int LVGF_DESCRIPTIONTOP = 0x400; // pszDescriptionTop is valid
    internal const int LVGF_DESCRIPTIONBOTTOM = 0x800; // pszDescriptionBottom is valid
    internal const int LVGF_TITLEIMAGE = 0x1000; // iTitleImage is valid
    internal const int LVGF_EXTENDEDIMAGE = 0x2000; // iExtendedImage is valid
    internal const int LVGF_ITEMS = 0x4000; // iFirstItem and cItems are valid
    internal const int LVGF_SUBSET = 0x8000; // pszSubsetTitle is valid
    internal const int LVGF_SUBSETITEMS = 0x10000; // readonly, cItems holds count of items in visible subset, iFirstItem is valid

    // Listview group styles
    internal const int LVGS_NORMAL = 0x0;
    internal const int LVGS_COLLAPSED = 0x1;
    internal const int LVGS_HIDDEN = 0x2;
    internal const int LVGS_NOHEADER = 0x4;
    internal const int LVGS_COLLAPSIBLE = 0x8;
    internal const int LVGS_FOCUSED = 0x10;
    internal const int LVGS_SELECTED = 0x20;
    internal const int LVGS_SUBSETED = 0x40;
    internal const int LVGS_SUBSETLINKFOCUSED = 0x80;

    internal const int LVGA_HEADER_LEFT = 0x1;
    internal const int LVGA_HEADER_CENTER = 0x2;
    internal const int LVGA_HEADER_RIGHT = 0x4; // Don't forget to validate exclusivity
    internal const int LVGA_FOOTER_LEFT = 0x8;
    internal const int LVGA_FOOTER_CENTER = 0x10;
    internal const int LVGA_FOOTER_RIGHT = 0x20; // Don't forget to validate exclusivity

    internal const int LVGGR_GROUP = 0; // Entire expanded group
    internal const int LVGGR_HEADER = 1;  // Header only (collapsed group)
    internal const int LVGGR_LABEL = 2;  // Label only
    internal const int LVGGR_SUBSETLINK = 3;  // subset link only

    /// <summary>
    /// constant to define dark mode option
    /// </summary>
    internal const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
    /// <summary>
    /// constant to define dark mode option
    /// </summary>
    internal const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
    /// <summary>
    /// Sent to a window to allow changes in that window to be redrawn, or to prevent changes in that window from being redrawn
    /// </summary>
    internal const int WM_SETREDRAW = 11;

    /// <summary>
    /// Sent when the window background must be erased (for example, when a window is resized).
    /// The message is sent to prepare an invalidated portion of a window for painting.
    /// </summary>
    internal const int WM_ERASEBKGND = 0x14;

    /// <summary>
    /// The WM_PAINT message is sent when the system or another application makes a request to paint a portion of an application's window. 
    /// </summary>
    internal const int WM_PAINT = 0xF;

    /// <summary>
    /// A static control, or an edit control that is read-only or disabled,
    /// sends the WM_CTLCOLORSTATIC message to its parent window when the control is about to be drawn.
    /// By responding to this message, the parent window can use the specified device context handle to set the text and
    /// background colors of the static control.
    /// </summary>
    internal const int WM_CTLCOLORSTATIC = 0x138;

    internal const int NM_FIRST = 0;
    internal const int NM_CLICK = NM_FIRST - 2;
    internal const int NM_CUSTOMDRAW = NM_FIRST - 12;

    /// <summary>
    /// MFC Message Reflection
    /// </summary>
    /// <remarks>See more: https://learn.microsoft.com/en-us/cpp/mfc/tn062-message-reflection-for-windows-controls</remarks>
    internal const int WM_REFLECT = 0x2000;

    /// <summary>
    /// Sent by a common control to its parent window when an event has occurred or the control requires some information.
    /// </summary>
    internal const int WM_NOFITY = 0x4E;

    /// <summary>
    /// Sent when the cursor is in an inactive window and the user presses a mouse button.
    /// </summary>
    internal const uint WM_MOUSEACTIVATE = 0x21;
    /// <summary>
    /// Return value from <see cref="WM_MOUSEACTIVATE"/>: Activates the window, and does not discard the mouse message
    /// </summary>
    internal const uint MA_ACTIVATE = 1;
    /// <summary>
    /// Return value from <see cref="WM_MOUSEACTIVATE"/>: Activates the window, and discards the mouse message
    /// </summary>
    internal const uint MA_ACTIVATEANDEAT = 2;


    /*
     * GetWindow() Constants
     */
    internal const int GW_HWNDFIRST = 0;
    internal const int GW_HWNDLAST = 1;
    internal const int GW_HWNDNEXT = 2;
    internal const int GW_HWNDPREV = 3;
    internal const int GW_OWNER = 4;
    internal const int GW_CHILD = 5;
}