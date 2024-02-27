using System.Runtime.InteropServices;
using StylableWinFormsControls.Native;
using static System.Windows.Forms.ListViewItem;

namespace StylableWinFormsControls;

public class StylableListView : ListView
{
    /// <summary>
    /// the settings to use
    /// </summary>
    private readonly WndProcErrorProcessor _errorProcessor;
    private Brush _groupHeaderBackColorBrush = new SolidBrush(Color.LightGray);

    /// <summary>
    /// Sets the color that build the background of any group header row
    /// </summary>
    public Color GroupHeaderBackColor
    {
        get => _groupHeaderBackColorBrush is SolidBrush solidBrush ? solidBrush.Color : default;
        set => _groupHeaderBackColorBrush = new SolidBrush(value);
    }

    private Brush _groupHeaderForeColorBrush = new SolidBrush(Color.Black);
    private Pen _groupHeaderForeColorPen = new(Color.Black);

    /// <summary>
    /// Sets the color that build the background of any group header row
    /// </summary>
    public Color GroupHeaderForeColor
    {
        get => _groupHeaderForeColorPen.Color;
        set
        {
            _groupHeaderForeColorBrush = new SolidBrush(value);
            _groupHeaderForeColorPen = new Pen(value);
        }
    }

    private Brush _selectedItemForeColorBrush = new SolidBrush(Color.Black);

    /// <summary>
    /// Sets the color that build the background of the selected row
    /// </summary>
    public Color SelectedItemForeColor
    {
        get => _selectedItemForeColorBrush is SolidBrush solidBrush ? solidBrush.Color : default;
        set => _selectedItemForeColorBrush = new SolidBrush(value);
    }

    private Brush _selectedItemBackColorBrush = new SolidBrush(Color.LightGray);

    /// <summary>
    /// Sets the color that build the background of the selected row
    /// </summary>
    public Color SelectedItemBackColor
    {
        get => _selectedItemBackColorBrush is SolidBrush solidBrush ? solidBrush.Color : default;
        set => _selectedItemBackColorBrush = new SolidBrush(value);
    }
    /// <summary>
    /// constructor
    /// </summary>
    public StylableListView() : this(StylableWinFormsControlsSettings.DEFAULT) { }
    /// <summary>
    /// this constructor can be used to override the default settings object in case some controls need separate settings
    /// or you use diffent libs all having a dependency on this control library.
    /// </summary>
    /// <param name="settings">the settings object to use</param>
    public StylableListView(StylableWinFormsControlsSettings settings)
    {
        _errorProcessor = new WndProcErrorProcessor(settings, wndProcInternal, base.WndProc);
        //Activate double buffering
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

        //Enable the OnNotifyMessage event so we get a chance to filter out
        // Windows messages before they get to the form's WndProc
        SetStyle(ControlStyles.EnableNotifyMessage, true);
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
        if (m.Msg != NativeConstants.Messages.WM_ERASEBKGND)
        {
            base.OnNotifyMessage(m);
        }
    }

    protected override void WndProc(ref Message m)
    {
        _errorProcessor.WndProc(ref m);
    }
    private void wndProcInternal(ref Message m)
    {
        if (m.Msg != NativeConstants.Messages.WM_REFLECT + NativeConstants.Messages.WM_NOFITY)
        {
            if (m.Msg == NativeConstants.Messages.WM_LBUTTONUP)
            {
                base.DefWndProc(ref m);
                return;
            }

            base.WndProc(ref m);
            return;
        }

        object? nmhdrParam = m.GetLParam(typeof(NativeMethods.NMHDR));
        if (nmhdrParam is null)
        {
            base.WndProc(ref m);
            return;
        }

        NativeMethods.NMHDR pnmhdr = (NativeMethods.NMHDR)nmhdrParam;
        if (pnmhdr.code != NativeConstants.NM_CUSTOMDRAW)
        {
            base.WndProc(ref m);
            return;
        }

        object? customDrawParam = m.GetLParam(typeof(NativeMethods.NMLVCUSTOMDRAW));
        if (customDrawParam is null)
        {
            base.WndProc(ref m);
            return;
        }

        NativeMethods.NMLVCUSTOMDRAW pnmlv = (NativeMethods.NMLVCUSTOMDRAW)customDrawParam;
        switch (pnmlv.nmcd.dwDrawStage)
        {
            case (int)NativeMethods.CDDS.PrePaint:
            {
                m.Result = pnmlv.dwItemType switch
                {
                    NativeConstants.LVCDI_GROUP => drawGroupHeader(m.HWnd, pnmlv),
                    _ => new IntPtr((int)NativeMethods.CDRF.NotifyItemDraw)
                };

                break;
            }
            case (int)NativeMethods.CDDS.ItemPrePaint:
            {
                switch (pnmlv.dwItemType)
                {
                    case NativeConstants.LVCDI_ITEM:
                        int itemIndex = (int)pnmlv.nmcd.dwItemSpec;

                        // skip items that are not selected as they are already drawn correctly
                        ListViewItem listViewItem = Items[itemIndex];
                        if (!listViewItem.Selected)
                        {
                            m.Result = new IntPtr((int)(NativeMethods.CDRF.NotifySubItemDraw |
                                                        NativeMethods.CDRF.NotifyPostPaint));
                            break;
                        }

                        m.Result = drawItem(m.HWnd, itemIndex, pnmlv);
                        break;

                    default:
                        m.Result = new IntPtr((int)(NativeMethods.CDRF.NotifySubItemDraw |
                                                    NativeMethods.CDRF.NotifyPostPaint));
                        break;
                }

                break;
            }
            case (int)NativeMethods.CDDS.ItemPostPaint:
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

        NativeMethods.SendMessageInternal(mHWnd, NativeConstants.Messages.LVM_GETITEMRECT, itemIndex, ref rectHeader);
        using (Graphics g = Graphics.FromHdc(pnmlv.nmcd.hdc))
        {
            // background color
            Rectangle rect = new(rectHeader.left, rectHeader.top,
                rectHeader.right - rectHeader.left, rectHeader.bottom - rectHeader.top);
            g.FillRectangle(_selectedItemBackColorBrush, rect);

            // item text
            ListViewItem item = Items[itemIndex];
            const int textOffset = 4;
            rect.Offset(textOffset, 1);
            g.DrawString(item.Text, Font, _selectedItemForeColorBrush, rect);
            rect.Offset(item.GetBounds(ItemBoundsPortion.Label).Width, 0);
            rect.Offset(textOffset, 0);

            // subitem text
            //the parent item is also the first subitem
            IEnumerable<ListViewSubItem> subList = (from ListViewSubItem subItem in item.SubItems select subItem).Skip(1);
            foreach (ListViewSubItem subItem in subList)
            {
                rect.Width = subItem.Bounds.Width;
                g.DrawString(subItem.Text, Font, _selectedItemForeColorBrush, rect);
                rect.Offset(subItem.Bounds.Width, 0);
                rect.Offset(textOffset, 0);
            }
        }

        return new IntPtr((int)NativeMethods.CDRF.SkipDefault);
    }

    private IntPtr drawGroupHeader(IntPtr mHWnd, NativeMethods.NMLVCUSTOMDRAW pnmlv)
    {
        //fix "failed to do native call 'SendMessage' (msg = LVM_GETGROUPINFO, wParam = 16)"
        if (pnmlv.nmcd.dwItemSpec == IntPtr.Zero)
        {
            return new IntPtr((int)NativeMethods.CDRF.SkipDefault);
        }
        NativeMethods.RECT rectHeader = new()
        {
            top = NativeConstants.LVGGR_HEADER
        };

        int groupIndex = (int)pnmlv.nmcd.dwItemSpec;

        NativeMethods.SendMessageInternal(mHWnd, NativeConstants.Messages.LVM_GETGROUPRECT, groupIndex,
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
            listviewGroup.mask = NativeConstants.LVGF_GROUPID | NativeConstants.LVGF_HEADER;

            NativeMethods.SendMessageInternal(
                mHWnd,
                NativeConstants.Messages.LVM_GETGROUPINFO,
                groupIndex,
                ref listviewGroup);

            string groupHeaderText = Marshal.PtrToStringUni(listviewGroup.pszHeader) ?? string.Empty;

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

        return new IntPtr((int)NativeMethods.CDRF.SkipDefault);
    }
}
