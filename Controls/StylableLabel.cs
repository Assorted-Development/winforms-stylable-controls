using System;
using System.Drawing;
using System.Windows.Forms;

namespace MFBot_1701_E.CustomControls
{
    // Based on original Label.cs code
    // Note that this doesn't support AutoEllipsis on disabled labels
    // Note that this doesn't support .NET Framework 1.0/1.1
    internal class StylableLabel : Label
    {
        MeasureTextCache _textMeasurementCache;
        MeasureTextCache MeasureTextCache => _textMeasurementCache ??= new MeasureTextCache();

        /// <summary>
        /// Gets or sets the foreground color if a label is disabled
        /// </summary>
        public Color DisabledForeColor { get; set; }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            // if the label isn't disabled, text rendering can be controlled via ForeColor
            if (Enabled)
            {
                base.OnPaint(e);
                return;
            }

            Rectangle face = DeflateRect(ClientRectangle, Padding);
            if (Image != null)
            {
                DrawImage(e.Graphics, Image, face, RtlTranslateAlignment(ImageAlign));
            }

            TextFormatFlags flags = CreateTextFormatFlags();
            TextRenderer.DrawText(e.Graphics, Text, Font, face, DisabledForeColor, flags);
        }

        private static Rectangle DeflateRect(Rectangle rect, Padding padding)
        {
            rect.X += padding.Left;
            rect.Y += padding.Top;
            rect.Width -= padding.Horizontal;
            rect.Height -= padding.Vertical;
            return rect;
        }

        private TextFormatFlags CreateTextFormatFlags()
        {
            return CreateTextFormatFlags(this.Size - GetBordersAndPadding());
        }

        private Size GetBordersAndPadding()
        {
            Size bordersAndPadding = Padding.Size;
            
            bordersAndPadding += SizeFromClientSize(Size.Empty);
            if (BorderStyle == BorderStyle.Fixed3D)
            {
                bordersAndPadding += new Size(2, 2);
            }

            return bordersAndPadding;

        }

        /// <devdoc>
        ///     Get TextFormatFlags flags for rendering text using GDI (TextRenderer).
        /// </devdoc>
        internal virtual TextFormatFlags CreateTextFormatFlags(Size constrainingSize)
        {
            TextFormatFlags flags = ControlPaintExtensions.CreateTextFormatFlags(this, TextAlign, AutoEllipsis, UseMnemonic);

            // Remove WordBreak if the size is large enough to display all the text.
            if (!MeasureTextCache.TextRequiresWordBreak(Text, Font, constrainingSize, flags))
            {
                // The effect of the TextBoxControl flag is that in-word line breaking will occur if needed, this happens when AutoSize 
                // is false and a one-word line still doesn't fit the binding box (width).  The other effect is that partially visible 
                // lines are clipped; this is how GDI+ works by default.
                flags &= ~(TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);
            }

            return flags;
        }
    }

    /// Source: MeasureTextCache in System.Windows.Forms.Layout.LayoutUtils (part of it)
    /// Cache mechanism added for VSWhidbey 500516
    /// 3000 character strings take 9 seconds to load the form
    internal class MeasureTextCache
    {
        private Size unconstrainedPreferredSize = InvalidSize;

        private static readonly Size MaxSize = new(Int32.MaxValue, Int32.MaxValue);
        private static readonly Size InvalidSize = new(Int32.MinValue, Int32.MinValue);

        /// InvalidateCache
        /// Clears out the cached values, should be called whenever Text, Font or a TextFormatFlag has changed
        public void InvalidateCache()
        {
            unconstrainedPreferredSize = InvalidSize;
        }

        /// TextRequiresWordBreak
        /// If you give the text all the space in the world it wants, then there should be no reason
        /// for it to break on a word.  So we find out what the unconstrained size is (Int32.MaxValue, Int32.MaxValue)
        /// for a string - eg. 35, 13.  If the size passed in has a larger width than 35, then we know that
        /// the WordBreak flag is not necessary.
        public bool TextRequiresWordBreak(string text, Font font, Size size, TextFormatFlags flags)
        {
            // if the unconstrained size of the string is larger than the proposed width
            // we need the word break flag, otherwise we dont, its a perf hit to use it.
            return GetUnconstrainedSize(text, font, flags).Width > size.Width;
        }

        /// GetUnconstrainedSize
        /// Gets the unconstrained (Int32.MaxValue, Int32.MaxValue) size for a piece of text
        private Size GetUnconstrainedSize(string text, Font font, TextFormatFlags flags)
        {
            if (unconstrainedPreferredSize == InvalidSize)
            {
                // we also investigated setting the SingleLine flag, however this did not yield as much benefit as the word break
                // and had possibility of causing internationalization issues.

                flags &= ~TextFormatFlags.WordBreak; // rip out the wordbreak flag
                unconstrainedPreferredSize = TextRenderer.MeasureText(text, font, MaxSize, flags);
            }

            return unconstrainedPreferredSize;
        }
    }
}
