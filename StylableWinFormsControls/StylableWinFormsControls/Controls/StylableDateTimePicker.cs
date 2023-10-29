using System.ComponentModel;
using System.Drawing.Text;
using System.Windows.Forms.VisualStyles;

namespace StylableWinFormsControls;

/// <summary>
/// the default datetime picker does not support styling
/// </summary>
public class StylableDateTimePicker : DateTimePicker
{
    public Color EnabledBackColor { get; set; } = Color.White;
    public Color DisabledBackColor { get; set; } = Color.Gray;
    public Color EnabledForeColor { get; set; } = Color.Black;
    public Color DisabledForeColor { get; set; } = Color.Black;

    public StylableDateTimePicker()
    {
        SetStyle(ControlStyles.UserPaint, true);
    }

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams handleParam = base.CreateParams;
            //prevent flickering of the control
            handleParam.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED
            return handleParam;
        }
    }

    /// <summary>
    ///  Gets or sets the background color of the control
    ///  </summary>
    [Browsable(true)]
    public override Color BackColor
    {
        get => base.BackColor;
        set => base.BackColor = value;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = CreateGraphics();
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        // Dropdownbutton rectangle
        Rectangle ddbRect = new(ClientRectangle.Width - 17, 0, 17, ClientRectangle.Height);
        // Background brush
        Brush bb;
        //foreground brush
        Brush fb;

        ComboBoxState visualState;

        // When enabled the brush is set to Backcolor,
        // otherwise to color stored in _disabled_back_Color
        if (Enabled)
        {
            bb = new SolidBrush(EnabledBackColor);
            fb = new SolidBrush(EnabledForeColor);
            visualState = ComboBoxState.Normal;
        }
        else
        {
            bb = new SolidBrush(DisabledBackColor);
            fb = new SolidBrush(DisabledForeColor);
            visualState = ComboBoxState.Disabled;
        }

        // Filling the background
        g.FillRectangle(bb, 0, 0, ClientRectangle.Width, ClientRectangle.Height);

        // Drawing the datetime text
        g.DrawString(Text, Font, fb, 5, 2);

        // Drawing the dropdownbutton using ComboBoxRenderer
        ComboBoxRenderer.DrawDropDownButton(g, ddbRect, visualState);

        g.Dispose();
        bb.Dispose();
        fb.Dispose();
    }
}
