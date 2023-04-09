using System;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using System.Windows.Forms;

namespace MFBot_1701_E.CustomControls
{
    internal class ControlPaintExtensions
    {
        // Taken from ControlPaint.DrawBackgroundImage, slightly modified to pass backColor as Brush
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
                throw new ArgumentNullException(nameof(g));

            if (backgroundImageLayout == ImageLayout.Tile)
            {
                using TextureBrush textureBrush = new TextureBrush(backgroundImage, WrapMode.Tile);

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
                    if (backgroundImageLayout == ImageLayout.Stretch || backgroundImageLayout == ImageLayout.Zoom)
                    {
                        imageRectangle.Intersect(clipRect);
                        g.DrawImage(backgroundImage, imageRectangle);
                    }
                    else if (backgroundImageLayout == ImageLayout.None)
                    {
                        imageRectangle.Offset(clipRect.Location);
                        Rectangle imageRect = imageRectangle;
                        imageRect.Intersect(clipRect);
                        Rectangle partOfImageToDraw = new Rectangle(Point.Empty, imageRect.Size);
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
                        Rectangle partOfImageToDraw = new Rectangle(
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
                    ImageAttributes imageAttrib = new ImageAttributes();
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
    }
}
