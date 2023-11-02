using System.Reflection;
using StylableWinFormsControls.Native;

namespace StylableWinFormsControls;

/// <summary>
/// the default combobox does not allow styling the background - only the background of the dropdown
/// </summary>
public class StylableComboBox : ComboBox
{
    /// <summary>
    /// the settings to use
    /// </summary>
    private readonly WndProcErrorProcessor _errorProcessor;
    private Pen _borderColorPen = new(SystemColors.ControlDark);

    /// <summary>
    /// Sets the color of the border around the combobox (not the item list box)
    /// </summary>
    public Color BorderColor
    {
        get => _borderColorPen.Color;
        set
        {
            _borderColorPen?.Dispose();
            _borderColorPen = new Pen(value, 2);
        }
    }

    private Brush _itemHoverColorBrush = new SolidBrush(SystemColors.Highlight);

    /// <summary>
    /// Sets the background color of the item in the list that's currently hovered/selected.
    /// </summary>
    public Color ItemHoverColor
    {
        get => _itemHoverColorBrush is SolidBrush solidBrush ? solidBrush.Color : default;
        set
        {
            _itemHoverColorBrush?.Dispose();
            _itemHoverColorBrush = new SolidBrush(value);
        }
    }

    private Brush _backColorBrush = new SolidBrush(DefaultBackColor);

    public override Color BackColor
    {
        get => base.BackColor;
        set
        {
            base.BackColor = value;
            _backColorBrush?.Dispose();
            _backColorBrush = new SolidBrush(value);
        }
    }

    private Brush _foreColorBrush = new SolidBrush(DefaultForeColor);

    public override Color ForeColor
    {
        get => base.ForeColor;
        set
        {
            base.ForeColor = value;
            _foreColorBrush?.Dispose();
            _foreColorBrush = new SolidBrush(value);
        }
    }

    /// <summary>
    /// constructor
    /// </summary>
    public StylableComboBox() : this(StylableWinFormsControlsSettings.DEFAULT) { }
    /// <summary>
    /// this constructor can be used to override the default settings object in case some controls need separate settings
    /// or you use diffent libs all having a dependency on this control library.
    /// </summary>
    /// <param name="settings">the settings object to use</param>
    public StylableComboBox(StylableWinFormsControlsSettings settings)
    {
        _errorProcessor = new WndProcErrorProcessor(settings, wndProcInternal, base.WndProc);
        DrawMode = DrawMode.OwnerDrawFixed;
        //calling SetStyle before the handle is created will cause erros like wrong fonts
        HandleCreated += (_, _) => SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _borderColorPen.Dispose();
            _itemHoverColorBrush.Dispose();
            _backColorBrush.Dispose();
            _foreColorBrush.Dispose();
        }

        base.Dispose(disposing);
    }

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        string value = string.Empty;
        if (e.Index >= 0)
        {
            object val = Items[e.Index];
            Type t = val.GetType();
            PropertyInfo? valProp = t.GetProperty(DisplayMember);

            value = valProp is not null
                ? valProp.GetValue(val)?.ToString() ?? string.Empty
                : Items[e.Index].ToString() ?? string.Empty;
        }

        Brush bgBrush = e.State.HasFlag(DrawItemState.Focus) ? _itemHoverColorBrush : _backColorBrush;
        e.Graphics.FillRectangle(bgBrush, e.Bounds);
        e.Graphics.DrawString(value, e.Font ?? Font, _foreColorBrush, e.Bounds, StringFormat.GenericDefault);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        if (DropDownStyle != ComboBoxStyle.Simple)
        {
            drawComboBox(e.Graphics);
            return;
        }

        base.OnPaint(e);
    }

    protected override void WndProc(ref Message m)
    {
        _errorProcessor.WndProc(ref m);
    }
    private void wndProcInternal(ref Message m)
    {
        switch (m.Msg)
        {
            //disabled box
            case NativeConstants.Messages.WM_CTLCOLORSTATIC:
                NativeMethods.SetBkColorInternal(m.WParam, ColorTranslator.ToWin32(Color.Orange));

                IntPtr brush = NativeMethods.CreateSolidBrush(ColorTranslator.ToWin32(Color.BlueViolet));
                m.Result = brush;
                return;

            case 0x133: //coloredit, for the edit area of editable comboboxes
                NativeMethods.SetBkModeInternal(m.WParam, NativeConstants.BKM_OPAQUE);
                NativeMethods.SetTextColorInternal(m.WParam, ColorTranslator.ToWin32(ForeColor));
                NativeMethods.SetBkColorInternal(m.WParam, ColorTranslator.ToWin32(BackColor));

                IntPtr brush0 = NativeMethods.CreateSolidBrush(ColorTranslator.ToWin32(BackColor));
                m.Result = brush0;
                return;

            case 0x134: //colorlistbox
                NativeMethods.SetBkModeInternal(m.WParam, NativeConstants.BKM_OPAQUE);
                NativeMethods.SetTextColorInternal(m.WParam, ColorTranslator.ToWin32(ForeColor));
                NativeMethods.SetBkColorInternal(m.WParam, ColorTranslator.ToWin32(BackColor));

                IntPtr brush2 = NativeMethods.CreateSolidBrush(ColorTranslator.ToWin32(BackColor));
                m.Result = brush2;
                return;
        }

        base.WndProc(ref m);
    }

    protected override void OnPaintBackground(PaintEventArgs pevent)
    {
        ArgumentNullException.ThrowIfNull(pevent);
        base.OnPaintBackground(pevent);

        if (DropDownStyle != ComboBoxStyle.DropDownList)
        {
            pevent.Graphics.DrawRectangle(_borderColorPen, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
        }
    }

    private Rectangle getDownRectangle()
    {
        return new Rectangle(ClientSize.Width - 16, 0, 16, ClientSize.Height);
    }

    private void drawComboBox(Graphics graphics)
    {
        Rectangle drawArea = ClientRectangle;
        using SolidBrush backBrush = new(BackColor);
        using SolidBrush foreBrush = new(ForeColor);

        using StringFormat stringFormat = new()
        {
            LineAlignment = StringAlignment.Center
        };

        graphics.FillRectangle(backBrush, drawArea);

        Rectangle textDrawArea = drawArea;
        textDrawArea.X += 4;
        graphics.DrawString(
            Text,
            Font,
            foreBrush,
            textDrawArea,
            stringFormat);

        ComboBoxRenderer.DrawDropDownButton(graphics, getDownRectangle(), System.Windows.Forms.VisualStyles.ComboBoxState.Normal);

        Rectangle borderRectangle = drawArea;
        graphics.DrawRectangle(_borderColorPen, borderRectangle);

        if (Focused && ShowFocusCues)
        {
            ControlPaint.DrawFocusRectangle(graphics, borderRectangle);
        }
    }
}
