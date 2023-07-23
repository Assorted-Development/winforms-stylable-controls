﻿using StylableWinFormsControls.Extensions;
using StylableWinFormsControls.LayoutInternals;

namespace StylableWinFormsControls;

// Based on original Label.cs code
// Note that this doesn't support AutoEllipsis on disabled labels
// Note that this doesn't support .NET Framework 1.0/1.1
public class StylableLabel : Label
{
    private MeasureTextCache _textMeasurementCache;
    private MeasureTextCache MeasureTextCache => _textMeasurementCache ??= new MeasureTextCache();

    /// <summary>
    /// Gets or sets the foreground color if a label is disabled
    /// </summary>
    public Color DisabledForeColor { get; set; }

    protected override void OnPaint(PaintEventArgs e)
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
        return CreateTextFormatFlags(Size - GetBordersAndPadding());
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