using System;
using System.Windows.Forms;
using MFBot_1701_E.Theming;

namespace MFBot_1701_E.CustomControls
{
    /// <summary>
    /// This class adds on to the functionality provided in System.Windows.Forms.ToolStrip.
    /// </summary>
    public class StylableToolStrip : ToolStrip
    {

        /// <summary>
        /// Gets or sets whether the ToolStripEx honors item clicks when its containing form does
        /// not have input focus.
        /// </summary>
        /// <remarks>
        /// Default value is false, which is the same behavior provided by the base ToolStrip class.
        /// </remarks>
        public bool ClickThrough { get; set; } = false;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (ClickThrough && m.Msg == NativeMethods.WM_MOUSEACTIVATE && m.Result == (IntPtr)NativeMethods.MA_ACTIVATEANDEAT)
                m.Result = (IntPtr)NativeMethods.MA_ACTIVATE;
        }
    }
}