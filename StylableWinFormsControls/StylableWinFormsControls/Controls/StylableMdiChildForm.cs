using StylableWinFormsControls.Utilities;
using System.Runtime.InteropServices;

namespace StylableWinFormsControls.Controls
{
    /// <summary>
    /// the default form does not align with the windows theme when used as an MDI child
    /// </summary>
    public partial class StylableMdiChildForm : Form
    {
        /// <summary>
        /// the width of the Controlbox
        /// </summary>
        private const int CONTROLBOX_WIDTH = 35;

        /// <summary>
        /// Sent to a window when its nonclient area needs to be changed to indicate an active or
        /// inactive state.
        /// </summary>
        private const int WM_NCACTIVATE = 0x0086;

        /// <summary>
        /// The WM_NCPAINT message is sent to a window when its frame must be painted.
        /// </summary>
        private const int WM_NCPAINT = 0x0085;

        private Brush _titleBrush = new SolidBrush(Color.FromArgb(32, 32, 32));

        private Color _titleColor = Color.FromArgb(32, 32, 32);

        /// <summary>
        /// defines the place where the icon is drawn
        /// </summary>
        private Rectangle _titleIconCanvas = new Rectangle(6, 6, 20, 20);

        /// <summary>
        /// constructor
        /// </summary>
        public StylableMdiChildForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        public Color BorderColor { get; set; } = Color.FromArgb(32, 32, 32);

        public Color TitleBackColor
        {
            get
            {
                return _titleColor;
            }
            set
            {
                _titleColor = value;
                _titleBrush = new SolidBrush(_titleColor);
            }
        }

        public Color TitleForeColor { get; set; } = Color.White;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (IsMdiChild && WindowState == FormWindowState.Maximized)
            {
                //no need to draw a titlebar or a border so we can rely on the normal Drawing
                return;
            }
            if (!IsMdiChild)
            {
                //when the form is not an MDI child, the form will be drawn by the Desktop Window Manager
                //so we don't need to draw it ourselves as the DWM correctly styles the form
                return;
            }
            if (m.Msg == WM_NCPAINT || m.Msg == WM_NCACTIVATE)
            {
                DrawFrame();
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("User32.dll")]
        private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        private void DrawFrame()
        {
            IntPtr hdc = GetWindowDC(this.Handle);
            using (Graphics g = Graphics.FromHdc(hdc))
            {
                g.DrawBorder(this);
                DrawTitleBar(g);
            }
            ReleaseDC(this.Handle, hdc);
        }

        private void DrawTitleBar(Graphics g)
        {
            //calculate controlbox width
            int controlBoxWidth = 0;
            if (ControlBox)
            {
                controlBoxWidth = CONTROLBOX_WIDTH + this.GetBorderWidth();
                if (MinimizeBox || MaximizeBox)
                {
                    //minimize/maximize are only hidden if both are disabled
                    controlBoxWidth += 2 * CONTROLBOX_WIDTH;
                }
            }
            //draw title bar background
            g.FillRectangle(_titleBrush, 0, 0, this.Width - controlBoxWidth, this.GetTitleBarHeight());

            //draw icon if available
            int offset = 0;
            if (this.Width > (controlBoxWidth + _titleIconCanvas.X + _titleIconCanvas.Width))
            {
                if (this.Icon != null)
                {
                    g.DrawIcon(this.Icon, _titleIconCanvas);
                    offset = _titleIconCanvas.X + _titleIconCanvas.Width;
                }
            }
            //draw titlebar text
            int widthAvailable = this.Width - controlBoxWidth - offset;
            Rectangle bounds = new Rectangle(offset, 6, widthAvailable, 20);
            TextRenderer.DrawText(g, Text, Font, bounds, TitleForeColor, TextFormatFlags.EndEllipsis);
        }
    }
}