using System.Windows.Forms.VisualStyles;
using StylableWinFormsControls.Native;

namespace StylableWinFormsControls;

/// <summary>
/// Represents a Windows <see cref="CheckBox"/>.
/// </summary>
/// <remarks>
/// Note: This control doesn't currently support <see cref="AutoSize"/>
/// </remarks>
public class StylableCheckBox : CheckBox
{
    private Rectangle _textRectangleValue;
    private bool _clicked;
    private CheckBoxState _state = CheckBoxState.UncheckedNormal;

    /// <summary>
    /// Indicates whether the control is automatically resized to fit its contents
    /// </summary>
    /// <remarks>
    /// AutoSize is currently automatically always set to false.
    /// </remarks>
    public override bool AutoSize
    {
        set => base.AutoSize = false;
        get => base.AutoSize;
    }

    /// <summary>
    /// Gets or sets the foreground color of the checkbox label if a checkbox is disabled
    /// </summary>
    public Color DisabledForeColor
    {
        get;
        set;
    }

    public StylableCheckBox()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
    }

    protected override void OnPaintBackground(PaintEventArgs pevent)
    {
        // skip because we draw it.
    }

    protected override void OnPaint(PaintEventArgs pevent)
    {
        ArgumentNullException.ThrowIfNull(pevent);

        drawCheckBox(pevent.Graphics);
    }

    protected override void WndProc(ref Message m)
    {
        // Filter out the WM_ERASEBKGND message since we do that on our own with foreground painting
        if (m.Msg == NativeConstants.Messages.WM_ERASEBKGND)
        {
            // return 0 (no erasing)
            m.Result = (IntPtr)1;

            return;
        }

        base.WndProc(ref m);
    }

    private void drawCheckBox(Graphics graphics)
    {
        Size glyphSize = CheckBoxRenderer.GetGlyphSize(graphics, _state);

        // Calculate the text bounds, excluding the check box.
        Rectangle textRectangle = getTextRectangle(glyphSize);

        // center box vertically with text, especially necessary for multiline,
        // but align if disabled because the glyph looks slightly different then.
        Rectangle glyphBounds = new(
            ClientRectangle.Location with
            {
                Y = textRectangle.Location.Y +
                    ((textRectangle.Height - textRectangle.Location.Y) / 2) -
                    (glyphSize.Height / 2)
            },
            glyphSize);

        // Paint over text since ít might look slightly offset
        // if the calculation between disabled and enabled control positions differ
        // and the previous checkbox doesn't get correctly erased (which happens sometimes)
        // And: Don't do Graphics.Clear, not good with RDP sessions
        using (Brush backBrush = new SolidBrush(BackColor))
        {
            graphics.FillRectangle(backBrush, ClientRectangle);
        }

        if (isMixed(_state))
        {
            ControlPaint.DrawMixedCheckBox(graphics, glyphBounds, convertToButtonState(_state) | ButtonState.Flat);
        }
        else
        {
            ControlPaint.DrawCheckBox(graphics, glyphBounds, convertToButtonState(_state) | ButtonState.Flat);
        }

        Color textColor = Enabled ? ForeColor : DisabledForeColor;

        TextRenderer.DrawText(
            graphics, Text, Font, textRectangle, textColor,
            TextFormatFlags.VerticalCenter);

        if (Focused && ShowFocusCues)
        {
            ControlPaint.DrawFocusRectangle(graphics, ClientRectangle);
        }
    }

    private Rectangle _oldClientRectangle = Rectangle.Empty;

    private Size _oldGlyphSize = Size.Empty;
    private Rectangle _textRectangle = Rectangle.Empty;

    private Rectangle getTextRectangle(Size glyphSize)
    {
        // don't spend unnecessary time on PInvokes
        if (_oldClientRectangle == ClientRectangle && _oldGlyphSize == glyphSize)
        {
            return _textRectangle;
        }

        _textRectangleValue.X = ClientRectangle.X +
                               glyphSize.Width +
                               3;

        _textRectangleValue.Y = ClientRectangle.Y;
        _textRectangleValue.Width = ClientRectangle.Width - glyphSize.Width;
        _textRectangleValue.Height = ClientRectangle.Height;

        _oldClientRectangle = ClientRectangle;
        _textRectangle = _textRectangleValue;
        _oldGlyphSize = glyphSize;

        return _textRectangleValue;
    }

    private static ButtonState convertToButtonState(CheckBoxState state)
    {
        return state switch
        {
            CheckBoxState.CheckedNormal or CheckBoxState.CheckedHot => ButtonState.Checked,
            CheckBoxState.CheckedPressed => ButtonState.Checked | ButtonState.Pushed,
            CheckBoxState.CheckedDisabled => ButtonState.Checked | ButtonState.Inactive,
            CheckBoxState.UncheckedPressed => ButtonState.Pushed,
            CheckBoxState.UncheckedDisabled => ButtonState.Inactive,
            //Downlevel mixed drawing works only if ButtonState.Checked is set
            CheckBoxState.MixedNormal or CheckBoxState.MixedHot => ButtonState.Checked,
            CheckBoxState.MixedPressed => ButtonState.Checked | ButtonState.Pushed,
            CheckBoxState.MixedDisabled => ButtonState.Checked | ButtonState.Inactive,
            _ => ButtonState.Normal,
        };
    }

    private static bool isMixed(CheckBoxState state)
    {
        return state switch
        {
            CheckBoxState.MixedNormal => true,
            CheckBoxState.MixedHot => true,
            CheckBoxState.MixedPressed => true,
            CheckBoxState.MixedDisabled => true,
            _ => false
        };
    }

    protected override void OnMouseDown(MouseEventArgs mevent)
    {
        // Draw the check box in the checked or unchecked state, alternately.
        if (!_clicked)
        {
            _clicked = true;
            _state = CheckBoxState.CheckedPressed;
        }
        else
        {
            _clicked = false;
            _state = CheckBoxState.UncheckedNormal;
        }

        base.OnMouseDown(mevent);
    }

    protected override void OnMouseHover(EventArgs e)
    {
        _state = _clicked ? CheckBoxState.CheckedHot : CheckBoxState.UncheckedHot;
        // Invalidate is unnecessary as long as we don't handle hovers visually
        //Invalidate();
        base.OnMouseHover(e);
    }

    protected override void OnMouseUp(MouseEventArgs mevent)
    {
        base.OnMouseUp(mevent);
        OnMouseHover(mevent);
    }

    protected override void OnMouseLeave(EventArgs eventargs)
    {
        // Draw the check box in the unpressed state.
        _state = _clicked ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal;
        base.OnMouseLeave(eventargs);
    }
}
