namespace AssortedDevelopment.StylableWinFormsControls
{
    /// <summary>
    /// the default WinForms Button does not allow styling the disabled version
    /// </summary>
    public class StylableButton : Button
    {
        public Color EnabledBackColor { get; set; } = Color.White;
        public Color EnabledHoverColor { get; set; } = Color.LightGray;
        public Color DisabledBackColor { get; set; } = Color.Gray;
        public Color EnabledForeColor { get; set; } = Color.Black;
        public Color DisabledForeColor { get; set; } = Color.Black;
        public Color BorderColor { get; set; } = Color.Black;

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!this.Enabled)
            {
                using (SolidBrush brush = new(DisabledBackColor))
                {
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                    TextRenderer.DrawText(e.Graphics, this.Text, this.Font, this.ClientRectangle, DisabledForeColor,
                        DisabledBackColor);

                    // border
                    Pen borderPen = new Pen(BorderColor, 1);
                    e.Graphics.DrawRectangle(borderPen, this.ClientRectangle.X, ClientRectangle.Y,
                        ClientRectangle.Width - 1, ClientRectangle.Height - 1);
                }
            }
            else
            {
                // background
                SolidBrush backBrush;
                bool MouseInControl = e.ClipRectangle.Contains(PointToClient(Cursor.Position));
                if (MouseInControl)
                {
                    backBrush = new SolidBrush(EnabledHoverColor);
                    e.Graphics.FillRectangle(backBrush, this.ClientRectangle);
                }
                else
                {
                    backBrush = new SolidBrush(EnabledBackColor);
                    e.Graphics.FillRectangle(backBrush, this.ClientRectangle);
                }

                if (BackgroundImage != null)
                {
                    // draw image, but leave a bit of margin to all sides
                    ControlPaintExtensions.DrawBackgroundImage(
                        e.Graphics,
                        BackgroundImage,
                        backBrush,
                        BackgroundImageLayout,
                        new Rectangle(ClientRectangle.X + 2, ClientRectangle.Y + 2, ClientRectangle.Width - 4, ClientRectangle.Height - 4),
                        new Rectangle(ClientRectangle.X + 2, ClientRectangle.Y + 2, ClientRectangle.Width - 4, ClientRectangle.Height - 4),
                        default,
                        RightToLeft);
                }
                else
                {
                    TextRenderer.DrawText(e.Graphics, this.Text, this.Font, this.ClientRectangle, EnabledForeColor,
                            backBrush.Color);
                }
                backBrush.Dispose();
                
                // border
                ControlPaint.DrawBorder(e.Graphics,
                    new Rectangle(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height),
                    BorderColor,
                    ButtonBorderStyle.Solid);
            }
        }
    }
}
