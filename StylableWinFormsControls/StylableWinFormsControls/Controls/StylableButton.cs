using StylableWinFormsControls.Extensions;

namespace StylableWinFormsControls;

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

    protected override void OnPaint(PaintEventArgs pevent)
    {
        ArgumentNullException.ThrowIfNull(pevent);

        if (!Enabled)
        {
            using SolidBrush brush = new(DisabledBackColor);
            pevent.Graphics.FillRectangle(brush, ClientRectangle);
            TextRenderer.DrawText(pevent.Graphics, Text, Font, ClientRectangle, DisabledForeColor,
                DisabledBackColor);

            // border
            using Pen borderPen = new(BorderColor, 1);
            pevent.Graphics.DrawRectangle(borderPen, ClientRectangle.X, ClientRectangle.Y,
                ClientRectangle.Width - 1, ClientRectangle.Height - 1);
        }
        else
        {
            // background
            SolidBrush backBrush;
            bool mouseInControl = pevent.ClipRectangle.Contains(PointToClient(Cursor.Position));
            if (mouseInControl)
            {
                backBrush = new SolidBrush(EnabledHoverColor);
                pevent.Graphics.FillRectangle(backBrush, ClientRectangle);
            }
            else
            {
                backBrush = new SolidBrush(EnabledBackColor);
                pevent.Graphics.FillRectangle(backBrush, ClientRectangle);
            }

            if (BackgroundImage is not null)
            {
                // draw image, but leave a bit of margin to all sides
                ControlPaintExtensions.DrawBackgroundImage(
                    pevent.Graphics,
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
                TextRenderer.DrawText(pevent.Graphics, Text, Font, ClientRectangle, EnabledForeColor,
                    backBrush.Color);
            }
            backBrush.Dispose();

            // border
            ControlPaint.DrawBorder(pevent.Graphics,
                new Rectangle(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height),
                BorderColor,
                ButtonBorderStyle.Solid);
        }
    }
}
