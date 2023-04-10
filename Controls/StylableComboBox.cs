using MFBot_1701_E.Theming;
using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace MFBot_1701_E.CustomControls
{
    /// <summary>
    /// the default combobox does not allow styling the background - only the background of the dropdown
    /// </summary>
    public class StylableComboBox : ComboBox
    {
        private Pen _borderColorPen = new(SystemColors.ControlDark);
        /// <summary>
        /// Border color of border in the tab control and around the tabs
        /// </summary>
        public Color BorderColor
        {
            set
            {
                _borderColorPen?.Dispose();
                _borderColorPen = new Pen(value, 2);
            }
        }

        public StylableComboBox()
        {
            this.DrawMode = DrawMode.OwnerDrawFixed;
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _borderColorPen?.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            string value = string.Empty;
            if (e.Index >= 0)
            {
                object val = Items[e.Index];
                Type t = val.GetType();
                PropertyInfo valProp = t.GetProperty(DisplayMember);

                value = valProp != null
                    ? valProp.GetValue(val)?.ToString() ?? string.Empty
                    : Items[e.Index].ToString();
            }

            SolidBrush bgBrush = e.State.HasFlag(DrawItemState.Focus) ? new SolidBrush(Color.Yellow) : new SolidBrush(BackColor);
            SolidBrush brush = new(ForeColor);
            e.Graphics.FillRectangle(bgBrush, e.Bounds);
            e.Graphics.DrawString(value, e.Font ?? Font, brush, e.Bounds, StringFormat.GenericDefault);
            
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.DropDownStyle == ComboBoxStyle.DropDownList)
            {
                drawComboBox(e.Graphics);
                return;
            }

            base.OnPaint(e);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                //disabled box
                case NativeMethods.WM_CTLCOLORSTATIC:
                    NativeMethods.SetBkColor(m.WParam, ColorTranslator.ToWin32(Color.Orange));

                    IntPtr brush = NativeMethods.CreateSolidBrush(ColorTranslator.ToWin32(Color.BlueViolet));
                    m.Result = brush;
                    return;
                case 0x133: //coloredit, for the edit area of editable comboboxes
                    NativeMethods.SetBkMode(m.WParam, NativeMethods.BKM_OPAQUE);
                    NativeMethods.SetTextColor(m.WParam, ColorTranslator.ToWin32(ForeColor));
                    NativeMethods.SetBkColor(m.WParam, ColorTranslator.ToWin32(BackColor));

                    IntPtr brush0 = NativeMethods.CreateSolidBrush(ColorTranslator.ToWin32(BackColor));
                    m.Result = brush0;
                    return;
                case 0x134: //colorlistbox
                    NativeMethods.SetBkMode(m.WParam, NativeMethods.BKM_OPAQUE);
                    NativeMethods.SetTextColor(m.WParam, ColorTranslator.ToWin32(ForeColor));
                    NativeMethods.SetBkColor(m.WParam, ColorTranslator.ToWin32(BackColor));

                    IntPtr brush2 = NativeMethods.CreateSolidBrush(ColorTranslator.ToWin32(BackColor));
                    m.Result = brush2;
                    return;
            }

            base.WndProc(ref m);
        }

        private Rectangle GetDownRectangle()
        {
            return new Rectangle(this.ClientSize.Width - 16, 0, 16, this.ClientSize.Height);
        }

        private void drawComboBox(Graphics graphics)
        {
            Rectangle drawArea = ClientRectangle;
            using SolidBrush backBrush = new(BackColor);
            using SolidBrush foreBrush = new(ForeColor);

            StringFormat stringFormat = new()
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
                textDrawArea, stringFormat);

            ComboBoxRenderer.DrawDropDownButton(graphics, GetDownRectangle(), System.Windows.Forms.VisualStyles.ComboBoxState.Normal);

            Rectangle borderRectangle = drawArea;
            graphics.DrawRectangle(_borderColorPen, borderRectangle);

            if (Focused && ShowFocusCues)
            {
                ControlPaint.DrawFocusRectangle(graphics, borderRectangle);
            }
        }
    }
}
