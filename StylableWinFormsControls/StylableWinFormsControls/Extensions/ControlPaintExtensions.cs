using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace StylableWinFormsControls.Extensions;

internal class ControlPaintExtensions
{
    //use these value to signify ANY of the right, top, left, center, or bottom alignments with the ContentAlignment enum.
    internal const ContentAlignment ANY_RIGHT_ALIGN = ContentAlignment.TopRight | ContentAlignment.MiddleRight | ContentAlignment.BottomRight;
    internal const ContentAlignment ANY_LEFT_ALIGN = ContentAlignment.TopLeft | ContentAlignment.MiddleLeft | ContentAlignment.BottomLeft;
    internal const ContentAlignment ANY_TOP_ALIGN = ContentAlignment.TopLeft | ContentAlignment.TopCenter | ContentAlignment.TopRight;
    internal const ContentAlignment ANY_BOTTOM_ALIGN = ContentAlignment.BottomLeft | ContentAlignment.BottomCenter | ContentAlignment.BottomRight;
    internal const ContentAlignment ANY_MIDDLE_ALIGN = ContentAlignment.MiddleLeft | ContentAlignment.MiddleCenter | ContentAlignment.MiddleRight;
    internal const ContentAlignment ANY_CENTER_ALIGN = ContentAlignment.TopCenter | ContentAlignment.MiddleCenter | ContentAlignment.BottomCenter;

    /// <remarks>Taken from ControlPaint.DrawBackgroundImage, slightly modified to pass backColor as Brush</remarks>
    /// <exception cref="ArgumentNullException"></exception>
    internal static void DrawBackgroundImage(
        Graphics g,
        Image backgroundImage,
        Brush backColorBrush,
        ImageLayout backgroundImageLayout,
        Rectangle bounds,
        Rectangle clipRect,
        Point scrollOffset = default,
        RightToLeft rightToLeft = RightToLeft.No)
    {
        if (g is null)
        {
            throw new ArgumentNullException(nameof(g));
        }

        if (backgroundImageLayout == ImageLayout.Tile)
        {
            using TextureBrush textureBrush = new(backgroundImage, WrapMode.Tile);

            // Make sure the brush origin matches the display rectangle, not the client rectangle,
            // so the background image scrolls on AutoScroll forms.
            if (scrollOffset != Point.Empty)
            {
                Matrix transform = textureBrush.Transform;
                transform.Translate(scrollOffset.X, scrollOffset.Y);
                textureBrush.Transform = transform;
            }

            g.FillRectangle(textureBrush, clipRect);
        }
        else
        {
            // Center, Stretch, Zoom

            Rectangle imageRectangle = CalculateBackgroundImageRectangle(bounds, backgroundImage, backgroundImageLayout);

            // Flip the coordinates only if we don't do any layout, since otherwise the image should be at the
            // center of the displayRectangle anyway.

            if (rightToLeft == RightToLeft.Yes && backgroundImageLayout == ImageLayout.None)
            {
                imageRectangle.X += clipRect.Width - imageRectangle.Width;
            }

            // We fill the entire cliprect with the backcolor in case the image is transparent.
            // Also, if gdi+ can't quite fill the rect with the image, they will interpolate the remaining
            // pixels, and make them semi-transparent. This is another reason why we need to fill the entire rect.
            // If we didn't where ever the image was transparent, we would get garbage.
            g.FillRectangle(backColorBrush, clipRect);

            if (!clipRect.Contains(imageRectangle))
            {
                if (backgroundImageLayout is ImageLayout.Stretch or ImageLayout.Zoom)
                {
                    imageRectangle.Intersect(clipRect);
                    g.DrawImage(backgroundImage, imageRectangle);
                }
                else if (backgroundImageLayout == ImageLayout.None)
                {
                    imageRectangle.Offset(clipRect.Location);
                    Rectangle imageRect = imageRectangle;
                    imageRect.Intersect(clipRect);
                    Rectangle partOfImageToDraw = new(Point.Empty, imageRect.Size);
                    g.DrawImage(
                        backgroundImage,
                        imageRect,
                        partOfImageToDraw.X,
                        partOfImageToDraw.Y,
                        partOfImageToDraw.Width,
                        partOfImageToDraw.Height,
                        GraphicsUnit.Pixel);
                }
                else
                {
                    Rectangle imageRect = imageRectangle;
                    imageRect.Intersect(clipRect);
                    Rectangle partOfImageToDraw = new(
                        new Point(imageRect.X - imageRectangle.X, imageRect.Y - imageRectangle.Y),
                        imageRect.Size);

                    g.DrawImage(
                        backgroundImage,
                        imageRect,
                        partOfImageToDraw.X,
                        partOfImageToDraw.Y,
                        partOfImageToDraw.Width,
                        partOfImageToDraw.Height,
                        GraphicsUnit.Pixel);
                }
            }
            else
            {
                ImageAttributes imageAttrib = new();
                imageAttrib.SetWrapMode(WrapMode.TileFlipXY);
                g.DrawImage(
                    backgroundImage,
                    imageRectangle,
                    0,
                    0,
                    backgroundImage.Width,
                    backgroundImage.Height,
                    GraphicsUnit.Pixel,
                    imageAttrib);

                imageAttrib.Dispose();
            }
        }
    }

    internal static Rectangle CalculateBackgroundImageRectangle(Rectangle bounds, Image backgroundImage, ImageLayout imageLayout)
    {
        Rectangle result = bounds;

        if (backgroundImage is null)
        {
            return result;
        }

        switch (imageLayout)
        {
            case ImageLayout.Stretch:
                result.Size = bounds.Size;
                break;

            case ImageLayout.None:
                result.Size = backgroundImage.Size;
                break;

            case ImageLayout.Center:
                result.Size = backgroundImage.Size;
                Size szCtl = bounds.Size;

                if (szCtl.Width > result.Width)
                {
                    result.X = (szCtl.Width - result.Width) / 2;
                }

                if (szCtl.Height > result.Height)
                {
                    result.Y = (szCtl.Height - result.Height) / 2;
                }

                break;

            case ImageLayout.Zoom:
                Size imageSize = backgroundImage.Size;
                float xRatio = bounds.Width / (float)imageSize.Width;
                float yRatio = bounds.Height / (float)imageSize.Height;
                if (xRatio < yRatio)
                {
                    // Width should fill the entire bounds.
                    result.Width = bounds.Width;

                    // Preserve the aspect ratio by multiplying the xRatio by the height, adding .5 to round to
                    // the nearest pixel.
                    result.Height = (int)((imageSize.Height * xRatio) + .5);
                    if (bounds.Y >= 0)
                    {
                        result.Y = (bounds.Height - result.Height) / 2;
                    }
                }
                else
                {
                    // Width should fill the entire bounds.
                    result.Height = bounds.Height;

                    // Preserve the aspect ratio by multiplying the xRatio by the height, adding .5 to round to
                    // the nearest pixel.
                    result.Width = (int)((imageSize.Width * yRatio) + .5);
                    if (bounds.X >= 0)
                    {
                        result.X = (bounds.Width - result.Width) / 2;
                    }
                }

                break;
        }

        return result;
    }

    internal static TextFormatFlags CreateTextFormatFlags(Control ctl, ContentAlignment textAlign, bool showEllipsis, bool useMnemonic)
    {
        textAlign = RtlTranslateContent(ctl, textAlign);
        TextFormatFlags flags = TextFormatFlagsForAlignmentGdi(textAlign);

        // The effect of the TextBoxControl flag is that in-word line breaking will occur if needed, this happens when AutoSize
        // is false and a one-word line still doesn't fit the binding box (width).  The other effect is that partially visible
        // lines are clipped; this is how GDI+ works by default.
        flags |= TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl;

        if (showEllipsis)
        {
            flags |= TextFormatFlags.EndEllipsis;
        }

        // Adjust string format for Rtl controls
        if (ctl.RightToLeft == RightToLeft.Yes)
        {
            flags |= TextFormatFlags.RightToLeft;
        }

        //if we don't use mnemonic, set formatFlag to NoPrefix as this will show the ampersand
        if (!useMnemonic)
        {
            flags |= TextFormatFlags.NoPrefix;
        }
        //else if we don't show keyboard cues, set formatFlag to HidePrefix as this will hide
        //the ampersand if we don't press down the alt key
        else if (/*!ctl.ShowKeyboardCues*/ false) // disabled for simplicity
        {
            flags |= TextFormatFlags.HidePrefix;
        }

        return flags;
    }

    internal static TextFormatFlags TextFormatFlagsForAlignmentGdi(ContentAlignment align)
    {
        TextFormatFlags output = new();
        output |= TranslateAlignmentForGdi(align);
        output |= TranslateLineAlignmentForGdi(align);
        return output;
    }

    internal static TextFormatFlags TranslateAlignmentForGdi(ContentAlignment align)
    {
        if ((align & ANY_BOTTOM_ALIGN) != 0)
        {
            return TextFormatFlags.Bottom;
        }

        if ((align & ANY_MIDDLE_ALIGN) != 0)
        {
            return TextFormatFlags.VerticalCenter;
        }

        return TextFormatFlags.Top;
    }

    internal static TextFormatFlags TranslateLineAlignmentForGdi(ContentAlignment align)
    {
        if ((align & ANY_RIGHT_ALIGN) != 0)
        {
            return TextFormatFlags.Right;
        }

        if ((align & ANY_CENTER_ALIGN) != 0)
        {
            return TextFormatFlags.HorizontalCenter;
        }
        return TextFormatFlags.Left;
    }

    internal static ContentAlignment RtlTranslateContent(Control ctl, ContentAlignment align)
    {
        if (RightToLeft.Yes == ctl.RightToLeft)
        {
            if ((align & ANY_TOP_ALIGN) != 0)
            {
                switch (align)
                {
                    case ContentAlignment.TopLeft:
                        return ContentAlignment.TopRight;

                    case ContentAlignment.TopRight:
                        return ContentAlignment.TopLeft;
                }
            }

            if ((align & ANY_MIDDLE_ALIGN) != 0)
            {
                switch (align)
                {
                    case ContentAlignment.MiddleLeft:
                        return ContentAlignment.MiddleRight;

                    case ContentAlignment.MiddleRight:
                        return ContentAlignment.MiddleLeft;
                }
            }

            if ((align & ANY_BOTTOM_ALIGN) != 0)
            {
                switch (align)
                {
                    case ContentAlignment.BottomLeft:
                        return ContentAlignment.BottomRight;

                    case ContentAlignment.BottomRight:
                        return ContentAlignment.BottomLeft;
                }
            }
        }

        return align;
    }
}
