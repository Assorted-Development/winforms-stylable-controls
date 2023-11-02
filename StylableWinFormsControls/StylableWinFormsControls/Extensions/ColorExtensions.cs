using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StylableWinFormsControls.Extensions
{
    /// <summary>
    /// Extensions for interaction with colors
    /// </summary>
    internal static class ColorExtensions
    {
        /// <summary>
        /// the middle value of the rgb range between light and dark:
        /// FF + FF + FF = 2FD
        /// 2FD /2 = 17E
        /// 17E => 382
        /// </summary>
        public const int RGB_MIDDLE_VALUE = 382;
        /// <summary>
        /// makes a color lighter
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        internal static Color Lighter(this Color c)
        {
            return ControlPaint.Light(c, 1f);
        }
        /// <summary>
        /// makes a color darker
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        internal static Color Darker(this Color c)
        {
            return ControlPaint.Dark(c, 1f);
        }
        /// <summary>
        /// highlights a color
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        internal static Color Highlight(this Color c)
        {
            //ignore alpha value in checking if we need to make it darker or lighter
            int rgb = c.R + c.G + c.B;
            if(rgb > RGB_MIDDLE_VALUE)
            {
                return c.Darker();
            }
            else
            {
                return c.Lighter();
            }
        }
    }
}
