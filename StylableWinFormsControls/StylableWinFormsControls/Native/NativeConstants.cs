using System.Globalization;
using System.Reflection;

namespace StylableWinFormsControls.Native
{
    /// <summary>
    /// contains constants required for native calls
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Roslynator",
        "RCS1181:Convert comment to documentation comment.",
        Justification = "Most of the comments are not documentation, but internal notes.")]
    internal static class NativeConstants
    {
        /// <summary>
        /// Background is filled with the current background color before the text, hatched brush, or pen is drawn.
        /// </summary>
        internal const int BKM_OPAQUE = 2;

        /// <summary>
        /// Background remains untouched.
        /// </summary>
        internal const int BKM_TRANSPARENT = 1;
        /// <summary>
        /// constant to define dark mode option
        /// </summary>
        internal const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        /// <summary>
        /// constant to define dark mode option
        /// </summary>
        internal const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20_H1 = 19;

        internal const int GW_CHILD = 5;
        internal const int GW_HWNDFIRST = 0;
        internal const int GW_HWNDLAST = 1;
        internal const int GW_HWNDNEXT = 2;
        internal const int GW_HWNDPREV = 3;
        internal const int GW_OWNER = 4;
        internal const int LVCDI_GROUP = 0x1;
        internal const int LVCDI_ITEM = 0x0;
        internal const int LVCDI_ITEMSLIST = 0x2;

        internal const int LVGA_FOOTER_CENTER = 0x10;
        internal const int LVGA_FOOTER_LEFT = 0x8;
        // Don't forget to validate exclusivity
        internal const int LVGA_FOOTER_RIGHT = 0x20;

        internal const int LVGA_HEADER_CENTER = 0x2;
        internal const int LVGA_HEADER_LEFT = 0x1;
        // Don't forget to validate exclusivity
        internal const int LVGA_HEADER_RIGHT = 0x4;

        internal const int LVGF_ALIGN = 0x8;
        // pszDescriptionBottom is valid
        internal const int LVGF_DESCRIPTIONBOTTOM = 0x800;

        // pszDescriptionTop is valid
        internal const int LVGF_DESCRIPTIONTOP = 0x400;

        // iExtendedImage is valid
        internal const int LVGF_EXTENDEDIMAGE = 0x2000;

        internal const int LVGF_FOOTER = 0x2;
        internal const int LVGF_GROUPID = 0x10;
        internal const int LVGF_HEADER = 0x1;
        // iFirstItem and cItems are valid
        internal const int LVGF_ITEMS = 0x4000;

        // Listview group specific flags
        internal const int LVGF_NONE = 0x0;

        internal const int LVGF_STATE = 0x4;
        // pszSubsetTitle is valid
        internal const int LVGF_SUBSET = 0x8000;

        // readonly, cItems holds count of items in visible subset, iFirstItem is valid
        internal const int LVGF_SUBSETITEMS = 0x10000;

        // pszSubtitle is valid
        internal const int LVGF_SUBTITLE = 0x100;

        // pszTask is valid
        internal const int LVGF_TASK = 0x200;

        // iTitleImage is valid
        internal const int LVGF_TITLEIMAGE = 0x1000;

        // Entire expanded group
        internal const int LVGGR_GROUP = 0;

        // Header only (collapsed group)
        internal const int LVGGR_HEADER = 1;

        // Label only
        internal const int LVGGR_LABEL = 2;

        // subset link only
        internal const int LVGGR_SUBSETLINK = 3;

        internal const int LVGS_COLLAPSED = 0x1;
        internal const int LVGS_COLLAPSIBLE = 0x8;
        internal const int LVGS_FOCUSED = 0x10;
        internal const int LVGS_HIDDEN = 0x2;
        internal const int LVGS_NOHEADER = 0x4;
        // Listview group styles
        internal const int LVGS_NORMAL = 0x0;

        internal const int LVGS_SELECTED = 0x20;
        internal const int LVGS_SUBSETED = 0x40;
        internal const int LVGS_SUBSETLINKFOCUSED = 0x80;
        internal const int LVM_FIRST = 0x1000;
        /// <summary>
        /// Return value from <see cref="WM_MOUSEACTIVATE"/>: Activates the window, and does not discard the mouse message
        /// </summary>
        internal const uint MA_ACTIVATE = 1;

        /// <summary>
        /// Return value from <see cref="WM_MOUSEACTIVATE"/>: Activates the window, and discards the mouse message
        /// </summary>
        internal const uint MA_ACTIVATEANDEAT = 2;

        internal const int NM_CLICK = NM_FIRST - 2;
        internal const int NM_CUSTOMDRAW = NM_FIRST - 12;
        internal const int NM_FIRST = 0;
        /*
         * GetWindow() Constants
         */
        internal static class Messages
        {
            internal const int LVM_GETGROUPINFO = LVM_FIRST + 149;
            internal const int LVM_GETGROUPRECT = LVM_FIRST + 98;
            internal const int LVM_GETITEMRECT = LVM_FIRST + 14;
            /// <summary>
            /// A static control, or an edit control that is read-only or disabled,
            /// sends the WM_CTLCOLORSTATIC message to its parent window when the control is about to be drawn.
            /// By responding to this message, the parent window can use the specified device context handle to set the text and
            /// background colors of the static control.
            /// </summary>
            internal const int WM_CTLCOLORSTATIC = 0x138;
            /// <summary>
            /// Sent when the window background must be erased (for example, when a window is resized).
            /// The message is sent to prepare an invalidated portion of a window for painting.
            /// </summary>
            internal const int WM_ERASEBKGND = 0x14;

            /// <summary>
            /// Posted when the user releases the left mouse button while the cursor is in the client area of a window.
            /// </summary>
            internal const int WM_LBUTTONUP = 0x202;
            /// <summary>
            /// Sent when the cursor is in an inactive window and the user presses a mouse button.
            /// </summary>
            internal const uint WM_MOUSEACTIVATE = 0x21;

            /// <summary>
            /// Sent by a common control to its parent window when an event has occurred or the control requires some information.
            /// </summary>
            internal const int WM_NOFITY = 0x4E;

            /// <summary>
            /// The WM_PAINT message is sent when the system or another application makes a request to paint a portion of an application's window.
            /// </summary>
            internal const int WM_PAINT = 0xF;

            /// <summary>
            /// MFC Message Reflection
            /// </summary>
            /// <remarks>See more: https://learn.microsoft.com/en-us/cpp/mfc/tn062-message-reflection-for-windows-controls</remarks>
            internal const int WM_REFLECT = 0x2000;

            /// <summary>
            /// Sent to a window to allow changes in that window to be redrawn, or to prevent changes in that window from being redrawn
            /// </summary>
            internal const int WM_SETREDRAW = 11;

            #region reverse msg value logic
            private static readonly Dictionary<long, string> MESSAGE_NAME_DICT = new();
            private static void initMessageNameFromValue()
            {
                //get all constants
                FieldInfo[] fieldInfos = typeof(Messages).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                List<FieldInfo> constants = (from f in fieldInfos where f.IsLiteral && !f.IsInitOnly && (typeof(int).IsAssignableFrom(f.FieldType) || typeof(uint).IsAssignableFrom(f.FieldType)) select f).ToList();


                //add all constants to the messageNameDict
                constants.ForEach(f =>
                {
                    object? value = f.GetValue(null);
                    if (value is not null)
                    {
                        MESSAGE_NAME_DICT.Add(Convert.ToInt64(value, CultureInfo.InvariantCulture), f.Name);
                    }
                }
                );
            }
            public static string Reverse(int value)
            {
                if (MESSAGE_NAME_DICT.Count == 0)
                {
                    initMessageNameFromValue();
                }
                if (MESSAGE_NAME_DICT.ContainsKey(value))
                {
                    return MESSAGE_NAME_DICT[value];
                }
                return $"Unknown Value({value})";
            }
            #endregion
        }
    }
}
