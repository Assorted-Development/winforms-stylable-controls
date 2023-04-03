using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MFBot_1701_E.CustomControls
{
    /// <summary>
    /// the default WinForms Button does not allow styling the disabled version
    /// </summary>
    public partial class StylableButton : Button
    {
        public Color EnabledBackColor { get; set; } = Color.White;
        public Color DisabledBackColor { get; set; } = Color.Gray;
        public Color EnabledForeColor { get; set; } = Color.Black;
        public Color DisabledForeColor { get; set; } = Color.Black;
        protected override void OnPaint(PaintEventArgs pevent)
        {
            if (!this.Enabled)
            {
                SolidBrush brush = new SolidBrush(DisabledBackColor);
                pevent.Graphics.FillRectangle(brush, this.ClientRectangle);
                TextRenderer.DrawText(pevent.Graphics, this.Text, this.Font, this.ClientRectangle, DisabledForeColor,
                    DisabledBackColor);
            }
            else
            {
                base.OnPaint(pevent);
            }
        }
    }
}
