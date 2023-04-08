using System.Drawing;
using System.Windows.Forms;

namespace MFBot_1701_E.CustomControls
{
    /// <summary>
    /// the default WinForms Button does not allow styling the disabled version
    /// </summary>
    public partial class StylableButton : Button
    {
        public Color EnabledBackColor { get; set; } = Color.White;
        public Color EnabledHoverColor { get; set; } = Color.LightGray;
        public Color DisabledBackColor { get; set; } = Color.Gray;
        public Color EnabledForeColor { get; set; } = Color.Black;
        public Color DisabledForeColor { get; set; } = Color.Black;
        public Color BorderColor { get; set; } = Color.Black;

        protected override void OnPaint(PaintEventArgs pevent)
        {
            if (!this.Enabled)
            {
                SolidBrush brush = new SolidBrush(DisabledBackColor);

                pevent.Graphics.FillRectangle(brush, this.ClientRectangle);
                TextRenderer.DrawText(pevent.Graphics, this.Text, this.Font, this.ClientRectangle, DisabledForeColor, DisabledBackColor);

                // border
                Pen borderPen = new Pen(BorderColor, 1);
                pevent.Graphics.DrawRectangle(borderPen, this.ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width - 1, ClientRectangle.Height - 1);
            }
            else
            {
                // background
                SolidBrush backBrush;
                bool MouseInControl = pevent.ClipRectangle.Contains(PointToClient(Cursor.Position));
                if (MouseInControl)
                {
                    backBrush = new SolidBrush(EnabledHoverColor);
                    pevent.Graphics.FillRectangle(backBrush, this.ClientRectangle);
                }
                else
                {
                    backBrush = new SolidBrush(EnabledBackColor);
                    pevent.Graphics.FillRectangle(backBrush, this.ClientRectangle);
                }
                
                TextRenderer.DrawText(pevent.Graphics, this.Text, this.Font, this.ClientRectangle, EnabledForeColor, backBrush.Color);

                // border
                Pen borderPen = new Pen(BorderColor, 1);
                pevent.Graphics.DrawRectangle(borderPen, this.ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width - 1, ClientRectangle.Height - 1);
            }
        }
    }
}
