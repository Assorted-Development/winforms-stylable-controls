using StylableWinFormsControls.Controls;

namespace StylableWinFormsControls.Utilities
{
    /// <summary>
    /// provides drawing logic for borders on a form
    /// </summary>
    internal static class CustomFormBorderStyleHandler
    {
        /// <summary>
        /// As the drawing is called often, we do not want to create the Brush every time so we cache them
        /// </summary>
        private static readonly Dictionary<Color, Brush> BRUSH_CACHE = new Dictionary<Color, Brush>();

        /// <summary>
        /// draw a border
        /// </summary>
        /// <param name="g">the graphics object to draw on</param>
        /// <param name="borderColor">the default color for the border. may be changed slightly (e.g. for 3D effects)</param>
        /// <param name="f">the form to get the settings from</param>
        public static void DrawBorder(this Graphics g, Color borderColor, Form f)
        {
            g.DrawBorder(borderColor, f.FormBorderStyle);
        }

        /// <summary>
        /// draw a border
        /// </summary>
        /// <param name="g">the graphics object to draw on</param>
        /// <param name="f">the form to get the settings from</param>
        public static void DrawBorder(this Graphics g, StylableMdiChildForm f)
        {
            g.DrawBorder(f.BorderColor, f);
        }

        /// <summary>
        /// draw a border
        /// </summary>
        /// <param name="g">the graphics object to draw on</param>
        /// <param name="borderColor">the default color for the border. may be changed slightly (e.g. for 3D effects)</param>
        /// <param name="style">the Border Style</param>
        /// <param name="topOffset">offset at the top. Can be used to allow space for a title bar</param>
        /// <param name="bottomOffset">offset at the bottom</param>
        /// <param name="leftOffset">offset at the left</param>
        /// <param name="rightOffset">offset at the right</param>
        public static void DrawBorder(this Graphics g, Color borderColor, FormBorderStyle style, int topOffset = 0, int bottomOffset = 0, int leftOffset = 0, int rightOffset = 0)
        {
            switch (style)
            {
                case FormBorderStyle.Fixed3D:
                //TODO: Implement
                case FormBorderStyle.Sizable:
                case FormBorderStyle.SizableToolWindow:
                case FormBorderStyle.FixedDialog:
                case FormBorderStyle.FixedToolWindow:
                case FormBorderStyle.FixedSingle:
                    DrawSimpleBorder(g, borderColor, topOffset, bottomOffset, leftOffset, rightOffset);
                    break;

                case FormBorderStyle.None:
                    //no border
                    break;
            }
        }

        /// <summary>
        /// returns a SolidBrush for the given Color
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Brush GetBrush(this Color c)
        {
            if (BRUSH_CACHE.ContainsKey(c))
            {
                return BRUSH_CACHE[c];
            }
            var brush = new SolidBrush(c);
            BRUSH_CACHE.Add(c, brush);
            return brush;
        }

        private static void DrawSimpleBorder(Graphics g, Color borderColor, int topOffset, int bottomOffset, int leftOffset, int rightOffset)
        {
            int totalHeight = (int)g.VisibleClipBounds.Height;
            int totalWidth = (int)g.VisibleClipBounds.Width;
            int borderWidth = 8;
            var borderBrush = borderColor.GetBrush();
            //draw left border
            g.FillRectangle(borderBrush, leftOffset, topOffset, borderWidth, totalHeight - topOffset - bottomOffset);
            //draw bottom border
            g.FillRectangle(borderBrush, leftOffset, totalHeight - bottomOffset - borderWidth, totalWidth - leftOffset - rightOffset, borderWidth);
            //draw right border
            g.FillRectangle(borderBrush, totalWidth - rightOffset - borderWidth, topOffset, borderWidth, totalHeight - topOffset - bottomOffset);
        }

        /// <summary>
        /// calculates the current border width on a form
        /// </summary>
        /// <param name="f">the form</param>
        /// <returns></returns>
        public static int GetBorderWidth(this Form f)
        {
            //setting the border style will update width and client size so we can calculate the width easily
            return (f.Width - f.ClientSize.Width) / 2;
        }

        /// <summary>
        /// calculates the height of the titlebar
        /// </summary>
        /// <param name="f">the form</param>
        /// <returns></returns>
        public static int GetTitleBarHeight(this Form f)
        {
            return f.Height - f.ClientSize.Height - f.GetBorderWidth();
        }
    }
}