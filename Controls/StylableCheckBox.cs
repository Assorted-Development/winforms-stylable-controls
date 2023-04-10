using MFBot_1701_E.Theming;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace MFBot_1701_E.CustomControls
{
    // note: this doesn't support AutoSize
    internal class StylableCheckBox : CheckBox
    {
        private Rectangle textRectangleValue;
        private bool clicked;
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

        public StylableCheckBox()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // skip because we draw it.
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            drawCheckBox(e.Graphics);
        }

        protected override void WndProc(ref Message m)
        {
            // Filter out the WM_ERASEBKGND message since we do that on our own with foreground painting
            if (m.Msg == NativeMethods.WM_ERASEBKGND)
            {
                // return 0 (no erasing)
                m.Result = (IntPtr)1;

                return;
            }

            base.WndProc(ref m);
        }
        
        private void drawCheckBox(Graphics graphics)
        {
            Size glyphSize = CheckBoxRenderer.GetGlyphSize(graphics, state);
            Rectangle textRectangle = GetTextRectangle(glyphSize);

            // center box vertically with text, especially necessary for multiline,
            // but align if disabled because the glyph looks slightly different then.
            Rectangle glyphBounds = new(
                ClientRectangle.Location with
                {
                    Y = textRectangle.Location.Y +
                        (textRectangle.Height - textRectangle.Location.Y) / 2 -
                        (glyphSize.Height / 2)
                },
                glyphSize);

            // Paint over text since ít might look slightly offset
            // if the calculation between disabled and enabled control positions differ
            // and the previous checkbox doesn't get correctly erased (which happens sometimes)
            // And: Don't do Graphics.Clear, not good with RDP sessions
            using (Brush backBrush = new SolidBrush(BackColor))
            {
                graphics.FillRectangle(backBrush, ClientRectangle);
            }

            if (IsMixed(state))
            {
                ControlPaint.DrawMixedCheckBox(graphics, glyphBounds, ConvertToButtonState(state) | ButtonState.Flat);
            }
            else
            {
                ControlPaint.DrawCheckBox(graphics, glyphBounds, ConvertToButtonState(state) | ButtonState.Flat);
            }

            Color textColor = Enabled ? ForeColor : DisabledForeColor;

            TextRenderer.DrawText(
                graphics, Text, Font, textRectangle, textColor,
                TextFormatFlags.VerticalCenter);

            if (Focused && ShowFocusCues)
            {
                ControlPaint.DrawFocusRectangle(graphics, ClientRectangle);
            }
        }

        // Calculate the text bounds, excluding the check box.
        private Rectangle _oldClientRectangle = Rectangle.Empty;
        private Size _oldGlyphSize = Size.Empty;
        private Rectangle _textRectangle = Rectangle.Empty;

        private Rectangle GetTextRectangle(Size glyphSize)
        {
            // don't spend unnecessary time on PInvokes
            if (_oldClientRectangle == ClientRectangle && _oldGlyphSize == glyphSize)
            {
                return _textRectangle;
            }

            textRectangleValue.X = ClientRectangle.X +
                                   glyphSize.Width +
                                   3;

            textRectangleValue.Y = ClientRectangle.Y;
            textRectangleValue.Width = ClientRectangle.Width - glyphSize.Width;
            textRectangleValue.Height = ClientRectangle.Height;

            _oldClientRectangle = ClientRectangle;
            _textRectangle = textRectangleValue;
            _oldGlyphSize = glyphSize;

            return textRectangleValue;
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
            if (!clicked)
            {
                clicked = true;
                state = CheckBoxState.CheckedPressed;
            }
            else
            {
                clicked = false;
                state = CheckBoxState.UncheckedNormal;
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseHover(EventArgs e)
        {
            state = clicked ? CheckBoxState.CheckedHot : CheckBoxState.UncheckedHot;
            // Invalidate is unnecessary as long as we don't handle hovers visually
            //Invalidate();
            base.OnMouseHover(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            OnMouseHover(e);
        }

        // Draw the check box in the unpressed state.
        protected override void OnMouseLeave(EventArgs e)
        {
            state = clicked ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal;
            base.OnMouseLeave(e);
        }
    }
}
