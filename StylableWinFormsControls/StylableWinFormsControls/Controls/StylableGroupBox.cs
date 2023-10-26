using System.ComponentModel;
using System.Windows.Forms.VisualStyles;

namespace StylableWinFormsControls.Controls
{
    /// <summary>
    /// the default GroupBox does not support changing the text/border color
    /// </summary>
    public class StylableGroupBox : GroupBox
    {
        /// <summary>
        /// the border color
        /// </summary>
        private Color _borderColor = Color.Black;
        /// <summary>
        /// the border color
        /// </summary>
        [DefaultValue(typeof(Color), "Black")]
        public Color BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(); }
        }
        /// <summary>
        /// the text color
        /// </summary>
        private Color _textColor = Color.Black;
        /// <summary>
        /// the text color
        /// </summary>
        [DefaultValue(typeof(Color), "Black")]
        public Color TextColor
        {
            get => _textColor;
            set { _textColor = value; Invalidate(); }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            TextFormatFlags flags = TextFormatFlags.PreserveGraphicsTranslateTransform |
                TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.TextBoxControl |
                TextFormatFlags.WordBreak;
            Color titleColor = TextColor;
            if (!ShowKeyboardCues)
            {
                flags |= TextFormatFlags.HidePrefix;
            }

            if (RightToLeft == RightToLeft.Yes)
            {
                flags |= TextFormatFlags.RightToLeft | TextFormatFlags.Right;
            }

            if (!Enabled)
            {
                titleColor = SystemColors.GrayText;
            }

            drawUnthemedGroupBoxWithText(e.Graphics, new Rectangle(0, 0, Width,
                Height), Text, Font, titleColor, flags);
            RaisePaintEvent(this, e);
        }
        private void drawUnthemedGroupBoxWithText(Graphics g, Rectangle bounds,
            string groupBoxText, Font font, Color titleColor,
            TextFormatFlags flags)
        {
            Rectangle rectangle = bounds;
            rectangle.Width -= 8;
            Size size = TextRenderer.MeasureText(g, groupBoxText, font,
                new Size(rectangle.Width, rectangle.Height), flags);
            rectangle.Width = size.Width;
            rectangle.Height = size.Height;
            if ((flags & TextFormatFlags.Right) == TextFormatFlags.Right)
            {
                rectangle.X = bounds.Right - rectangle.Width - 8;
            }
            else
            {
                rectangle.X += 8;
            }

            TextRenderer.DrawText(g, groupBoxText, font, rectangle, titleColor, flags);
            if (rectangle.Width > 0)
            {
                rectangle.Inflate(2, 0);
            }

            using Pen pen = new(BorderColor);
            int num = bounds.Top + font.Height / 2;
            g.DrawLine(pen, bounds.Left, num - 1, bounds.Left, bounds.Height - 2);
            g.DrawLine(pen, bounds.Left, bounds.Height - 2, bounds.Width - 1,
                bounds.Height - 2);
            g.DrawLine(pen, bounds.Left, num - 1, rectangle.X - 3, num - 1);
            g.DrawLine(pen, rectangle.X + rectangle.Width + 2, num - 1,
                bounds.Width - 2, num - 1);
            g.DrawLine(pen, bounds.Width - 2, num - 1, bounds.Width - 2,
               bounds.Height - 2);
        }
    }
}
