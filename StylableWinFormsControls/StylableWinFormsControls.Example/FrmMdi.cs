using StylableWinFormsControls.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StylableWinFormsControls.Example
{
    public partial class FrmMdi : Form
    {
        /// <summary>
        /// constant to define dark mode option
        /// </summary>
        internal const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        /// constant to define dark mode option
        /// </summary>
        internal const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;

        /// <summary>
        /// native method to set the title bar style
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="attr"></param>
        /// <param name="attrValue"></param>
        /// <param name="attrSize"></param>
        /// <returns></returns>
        [DllImport("dwmapi.dll")]
        internal static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("uxtheme.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr OpenThemeData(IntPtr hWnd, string classList);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        internal static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

        public FrmMdi()
        {
            InitializeComponent();
            SetWindowTheme(this.Handle, "DarkMode_Explorer", null);
            OpenThemeData(IntPtr.Zero, "Explorer::ScrollBar");
            int useImmersiveDarkMode = 1;
            DwmSetWindowAttribute(this.Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref useImmersiveDarkMode, sizeof(int));
        }
        public FrmMdi(bool parent = true): this()
        {
            if (parent)
            {
                this.IsMdiContainer = true;
                var child = new StylableMdiChildForm();
                child.TitleHeight = 40;
                child.MdiParent = this;
                child.Show();
            }
        }
    }
}
