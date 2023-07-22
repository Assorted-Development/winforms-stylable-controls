using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace MFBot_1701_E.CustomControls
{
    /// <summary>
    /// the default datetime picker does not support styling
    /// </summary>
    public class StyleableDateTimePicker : DateTimePicker
    {
        public Color EnabledBackColor { get; set; } = Color.White;
        public Color DisabledBackColor { get; set; } = Color.Gray;
        public Color EnabledForeColor { get; set; } = Color.Black;
        public Color DisabledForeColor { get; set; } = Color.Black;
        public StyleableDateTimePicker()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
        }
        /// <summary>
        ///  Gets or sets the background color of the control
        ///  </summary>
        [Browsable(true)]
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
            }
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            Graphics g = this.CreateGraphics();
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Dropdownbutton rectangle
            Rectangle ddb_rect = new Rectangle(ClientRectangle.Width - 17, 0, 17, ClientRectangle.Height);
            // Background brush
            Brush bb;
            //foreground brush
            Brush fb;

            ComboBoxState visual_state;

            // When enabled the brush is set to Backcolor, 
            // otherwise to color stored in _disabled_back_Color
            if (this.Enabled)
            {
                bb = new SolidBrush(EnabledBackColor);
                fb = new SolidBrush(EnabledForeColor);
                visual_state = ComboBoxState.Normal;
            }
            else
            {
                bb = new SolidBrush(DisabledBackColor);
                fb = new SolidBrush(DisabledForeColor);
                visual_state = ComboBoxState.Disabled;
            }

            // Filling the background
            g.FillRectangle(bb, 0, 0, ClientRectangle.Width, ClientRectangle.Height);

            // Drawing the datetime text
            g.DrawString(this.Text, this.Font, fb, 5, 2);

            // Drawing the dropdownbutton using ComboBoxRenderer
            ComboBoxRenderer.DrawDropDownButton(g, ddb_rect, visual_state);

            g.Dispose();
            bb.Dispose();
            fb.Dispose();
        }
    }
}
