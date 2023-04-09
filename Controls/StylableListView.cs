using System.Drawing.Drawing2D;
using System.Drawing;
using System.Runtime.InteropServices;
using System;
using System.Windows.Forms;
using MFBot_1701_E.Theming;

namespace MFBot_1701_E.CustomControls
{
    internal class StylableListView : ListView
    {
        private Brush _groupHeaderBackColorBrush = new SolidBrush(Color.Transparent);

        /// <summary>
        /// Sets the color that build the background of any group header row
        /// </summary>
        public Color GroupHeaderBackColor
        {
            set => _groupHeaderBackColorBrush = new SolidBrush(value);
        }

        private Brush _groupHeaderForeColorBrush = new SolidBrush(Color.Orange);
        private Pen _groupHeaderForeColorPen = new(Color.Orange);

        /// <summary>
        /// Sets the color that build the background of any group header row
        /// </summary>
        public Color GroupHeaderForeColor
        {
            set
            {
                _groupHeaderForeColorBrush = new SolidBrush(value);
                _groupHeaderForeColorPen = new Pen(value);
            }
        }

        public StylableListView()
        {
            //Activate double buffering
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            //Enable the OnNotifyMessage event so we get a chance to filter out 
            // Windows messages before they get to the form's WndProc
            this.SetStyle(ControlStyles.EnableNotifyMessage, true);
        }

        protected override void OnNotifyMessage(Message m)
        {
            //Filter out the WM_ERASEBKGND message
            if (m.Msg != 0x14)
            {
                base.OnNotifyMessage(m);
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg != NativeMethods.WM_REFLECT + NativeMethods.WM_NOFITY)
            {
                if (m.Msg == NativeMethods.WM_LBUTTONUP)
                {
                    base.DefWndProc(ref m);
                    return;
                }

                base.WndProc(ref m);
                return;
            }

            NativeMethods.NMHDR pnmhdr = (NativeMethods.NMHDR)m.GetLParam(typeof(NativeMethods.NMHDR));
            if (pnmhdr.code != NativeMethods.NM_CUSTOMDRAW)
            {
                base.WndProc(ref m);
                return;
            }

            NativeMethods.NMLVCUSTOMDRAW pnmlv = (NativeMethods.NMLVCUSTOMDRAW)m.GetLParam(typeof(NativeMethods.NMLVCUSTOMDRAW));
            switch (pnmlv.nmcd.dwDrawStage)
            {
                case (int)NativeMethods.CDDS.CDDS_PREPAINTField:
                    {
                        if (pnmlv.dwItemType != NativeMethods.LVCDI_GROUP)
                        {
                            m.Result = new IntPtr((int)NativeMethods.CDRF.CDRF_NOTIFYITEMDRAWField);
                            break;
                        }

                        NativeMethods.RECT rectHeader = new()
                        {
                            top = NativeMethods.LVGGR_HEADER
                        };

                        int groupIndex = (int)pnmlv.nmcd.dwItemSpec;

                        NativeMethods.SendMessage(m.HWnd, NativeMethods.LVM_GETGROUPRECT, groupIndex, ref rectHeader);
                        using (Graphics g = Graphics.FromHdc(pnmlv.nmcd.hdc))
                        {
                            // Background color
                            Rectangle rect = new(rectHeader.left, rectHeader.top,
                                rectHeader.right - rectHeader.left, rectHeader.bottom - rectHeader.top);
                            g.FillRectangle(_groupHeaderBackColorBrush, rect);

                            // Group header text
                            NativeMethods.LVGROUP listviewGroup = new();
                            listviewGroup.cbSize = (uint)Marshal.SizeOf(listviewGroup);
                            listviewGroup.mask = NativeMethods.LVGF_STATE | NativeMethods.LVGF_GROUPID |
                                                 NativeMethods.LVGF_HEADER;
                            NativeMethods.SendMessage(m.HWnd, NativeMethods.LVM_GETGROUPINFO, groupIndex,
                                ref listviewGroup);
                            string groupHeaderText = Marshal.PtrToStringUni(listviewGroup.pszHeader);

                            const int textOffset = 10;
                            rect.Offset(textOffset, 2);
                            g.DrawString(groupHeaderText, Font, _groupHeaderForeColorBrush, rect);

                            // Divider line
                            SizeF stringSize = g.MeasureString(groupHeaderText, Font);
                            int headerCenterY = rectHeader.top + ((rectHeader.bottom - rectHeader.top) / 2);

                            g.DrawLine(
                                _groupHeaderForeColorPen,
                                stringSize.Width + textOffset + 3, headerCenterY,
                                rectHeader.right - 10, headerCenterY);
                        }

                        m.Result = new IntPtr((int)NativeMethods.CDRF.CDRF_SKIPDEFAULTField);

                        break;
                    }
                case (int)NativeMethods.CDDS.CDDS_ITEMPREPAINTField:
                    {
                        m.Result = new IntPtr((int)(NativeMethods.CDRF.CDRF_NOTIFYSUBITEMDRAWField |
                                                    NativeMethods.CDRF.CDRF_NOTIFYPOSTPAINTField));
                        break;
                    }
                case (int)NativeMethods.CDDS.CDDS_ITEMPOSTPAINTField:
                    {
                        break;
                    }
            }
        }
    }
}
