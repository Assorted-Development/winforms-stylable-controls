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

        private Brush _selectedItemForeColorBrush = new SolidBrush(Color.Orange);

        /// <summary>
        /// Sets the color that build the background of any group header row
        /// </summary>
        public Color SelectedItemForeColor
        {
            set => _selectedItemForeColorBrush = new SolidBrush(value);
        }

        private Brush _selectedItemBackColorBrush = new SolidBrush(Color.Orange);

        /// <summary>
        /// Sets the color that build the background of any group header row
        /// </summary>
        public Color SelectedItemBackColor
        {
            set => _selectedItemBackColorBrush = new SolidBrush(value);
        }

        public StylableListView()
        {
            //Activate double buffering
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            //Enable the OnNotifyMessage event so we get a chance to filter out 
            // Windows messages before they get to the form's WndProc
            this.SetStyle(ControlStyles.EnableNotifyMessage, true);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _groupHeaderBackColorBrush?.Dispose();
                _groupHeaderForeColorBrush?.Dispose();
                _groupHeaderForeColorPen?.Dispose();
                _selectedItemForeColorBrush?.Dispose();
                _selectedItemBackColorBrush?.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override void OnNotifyMessage(Message m)
        {
            //Filter out the WM_ERASEBKGND message
            if (m.Msg != NativeMethods.WM_ERASEBKGND)
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
                        switch (pnmlv.dwItemType)
                        {
                            case NativeMethods.LVCDI_GROUP:
                                m.Result = drawGroupHeader(m.HWnd, pnmlv);
                                break;
                            default:
                                m.Result = new IntPtr((int)NativeMethods.CDRF.CDRF_NOTIFYITEMDRAWField);
                                break;
                        }

                        break;
                    }
                case (int)NativeMethods.CDDS.CDDS_ITEMPREPAINTField:
                    {
                        switch (pnmlv.dwItemType)
                        {
                            case NativeMethods.LVCDI_ITEM:
                                int itemIndex = (int)pnmlv.nmcd.dwItemSpec;

                                // skip items that are not selected as they are already drawn correctly
                                ListViewItem listViewItem = Items[itemIndex];
                                if (!listViewItem.Selected)
                                {
                                    m.Result = new IntPtr((int)(NativeMethods.CDRF.CDRF_NOTIFYSUBITEMDRAWField |
                                                                NativeMethods.CDRF.CDRF_NOTIFYPOSTPAINTField));
                                    break;
                                }

                                m.Result = drawItem(m.HWnd, itemIndex, pnmlv);
                                break;
                            default:
                                m.Result = new IntPtr((int)(NativeMethods.CDRF.CDRF_NOTIFYSUBITEMDRAWField |
                                                            NativeMethods.CDRF.CDRF_NOTIFYPOSTPAINTField));
                                break;
                        }

                        break;
                    }
                case (int)NativeMethods.CDDS.CDDS_ITEMPOSTPAINTField:
                    {
                        break;
                    }
            }
        }

        private IntPtr drawItem(IntPtr mHWnd, int itemIndex, NativeMethods.NMLVCUSTOMDRAW pnmlv)
        {
            NativeMethods.RECT rectHeader = new()
            {
                left = (int)ItemBoundsPortion.Entire
            };

            NativeMethods.SendMessage(mHWnd, NativeMethods.LVM_GETITEMRECT, itemIndex, ref rectHeader);
            using (Graphics g = Graphics.FromHdc(pnmlv.nmcd.hdc))
            {
                // background color
                Rectangle rect = new(rectHeader.left, rectHeader.top,
                    rectHeader.right - rectHeader.left, rectHeader.bottom - rectHeader.top);
                g.FillRectangle(_selectedItemBackColorBrush, rect);

                // item text
                const int textOffset = 4;
                rect.Offset(textOffset, 1);
                g.DrawString(Items[itemIndex].Text, Font, _selectedItemForeColorBrush, rect);
            }

            return new IntPtr((int)NativeMethods.CDRF.CDRF_SKIPDEFAULTField);
        }
        private IntPtr drawGroupHeader(IntPtr mHWnd, NativeMethods.NMLVCUSTOMDRAW pnmlv)
        {
            NativeMethods.RECT rectHeader = new()
            {
                top = NativeMethods.LVGGR_HEADER
            };

            int groupIndex = (int)pnmlv.nmcd.dwItemSpec;

            NativeMethods.SendMessage(mHWnd, NativeMethods.LVM_GETGROUPRECT, groupIndex,
                ref rectHeader);
            using (Graphics g = Graphics.FromHdc(pnmlv.nmcd.hdc))
            {
                // Background color
                Rectangle rect = new(rectHeader.left, rectHeader.top,
                    rectHeader.right - rectHeader.left, rectHeader.bottom - rectHeader.top);
                g.FillRectangle(_groupHeaderBackColorBrush, rect);

                // Group header text
                NativeMethods.LVGROUP listviewGroup = new();
                listviewGroup.cbSize = (uint)Marshal.SizeOf(listviewGroup);
                listviewGroup.mask = NativeMethods.LVGF_GROUPID | NativeMethods.LVGF_HEADER;

                NativeMethods.SendMessage(mHWnd, NativeMethods.LVM_GETGROUPINFO, groupIndex,
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

            return new IntPtr((int)NativeMethods.CDRF.CDRF_SKIPDEFAULTField);
        }
    }
}
