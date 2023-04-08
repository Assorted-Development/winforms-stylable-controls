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
        public StylableComboBox()
        {
            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.DrawItem += StylableComboBox_DrawItem;
        }

        private void StylableComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            string value = "";
            if(e.Index >= 0)
            {
                object val = Items[e.Index];
                Type t = val.GetType();
                PropertyInfo valProp = t.GetProperty("Value");
                if(valProp != null)
                {
                    value = valProp.GetValue(val).ToString();
                }
                else
                {
                    value = Items[e.Index].ToString();
                }
            }
            SolidBrush bgBrush = new SolidBrush(BackColor);
            SolidBrush brush = new SolidBrush(ForeColor);
            e.Graphics.FillRectangle(bgBrush, this.ClientRectangle);
            e.Graphics.DrawString(value, e.Font, brush, e.Bounds, StringFormat.GenericDefault);
            e.DrawFocusRectangle();
        }
    }
}
