using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace MFBot_1701_E.CustomControls
{
    // modified by MFBot team
    // note: this doesn't support AutoSize
    internal class StylableCheckBox : CheckBox
    {
        private Rectangle textRectangleValue = new();
        private bool clicked = false;
        private CheckBoxState state = CheckBoxState.UncheckedNormal;


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

        public override Color ForeColor
        {
            get;
            set;
        }

        public StylableCheckBox()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        // Calculate the text bounds, excluding the check box.
        public Rectangle TextRectangle
        {
            get
            {
                using (Graphics g = this.CreateGraphics())
                {
                    textRectangleValue.X = ClientRectangle.X +
                        CheckBoxRenderer.GetGlyphSize(g, CheckBoxState.UncheckedNormal).Width +
                        3;
                    textRectangleValue.Y = ClientRectangle.Y;
                    textRectangleValue.Width = ClientRectangle.Width -
                        CheckBoxRenderer.GetGlyphSize(g, CheckBoxState.UncheckedNormal).Width;
                    textRectangleValue.Height = ClientRectangle.Height;
                }

                return textRectangleValue;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(BackColor);

            Rectangle textRectangle = TextRectangle;

            Size glyphSize = CheckBoxRenderer.GetGlyphSize(e.Graphics, state);

            // center box vertically with text, especially necessary for multiline
            Rectangle glyphBounds = new(
                ClientRectangle.Location with { Y = textRectangle.Location.Y +
                                                    (textRectangle.Height - textRectangle.Location.Y) / 2 -
                                                    (glyphSize.Height / 2) },
                glyphSize);

            if (IsMixed(state))
            {
                ControlPaint.DrawMixedCheckBox(e.Graphics, glyphBounds, ConvertToButtonState(state));
            }
            else
            {
                ControlPaint.DrawCheckBox(e.Graphics, glyphBounds, ConvertToButtonState(state));
            }

            Color textColor = Enabled ? ForeColor : DisabledForeColor;

            TextRenderer.DrawText(
                e.Graphics, Text, Font, textRectangle, textColor,
                TextFormatFlags.VerticalCenter);

            if (ShowFocusCues && Focused)
            {
                ControlPaint.DrawFocusRectangle(e.Graphics, ClientRectangle);
            }
        }

        private static ButtonState ConvertToButtonState(CheckBoxState state)
        {
            switch (state)
            {
                case CheckBoxState.CheckedNormal:
                case CheckBoxState.CheckedHot:
                    return ButtonState.Checked;
                case CheckBoxState.CheckedPressed:
                    return (ButtonState.Checked | ButtonState.Pushed);
                case CheckBoxState.CheckedDisabled:
                    return (ButtonState.Checked | ButtonState.Inactive);

                case CheckBoxState.UncheckedPressed:
                    return ButtonState.Pushed;
                case CheckBoxState.UncheckedDisabled:
                    return ButtonState.Inactive;

                //Downlevel mixed drawing works only if ButtonState.Checked is set
                case CheckBoxState.MixedNormal:
                case CheckBoxState.MixedHot:
                    return ButtonState.Checked;
                case CheckBoxState.MixedPressed:
                    return (ButtonState.Checked | ButtonState.Pushed);
                case CheckBoxState.MixedDisabled:
                    return (ButtonState.Checked | ButtonState.Inactive);

                default:
                    return ButtonState.Normal;
            }
        }

        private static bool IsMixed(CheckBoxState state)
        {
            switch (state)
            {
                case CheckBoxState.MixedNormal:
                case CheckBoxState.MixedHot:
                case CheckBoxState.MixedPressed:
                case CheckBoxState.MixedDisabled:
                    return true;

                default:
                    return false;
            }
        }
        // Draw the check box in the checked or unchecked state, alternately.
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (!clicked)
            {
                clicked = true;
                state = CheckBoxState.CheckedPressed;
                Invalidate();
            }
            else
            {
                clicked = false;
                state = CheckBoxState.UncheckedNormal;
                Invalidate();
            }
        }

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);
            state = clicked ? CheckBoxState.CheckedHot : CheckBoxState.UncheckedHot;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            this.OnMouseHover(e);
        }

        // Draw the check box in the unpressed state.
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            state = clicked ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal;
            Invalidate();
        }
    }
}
