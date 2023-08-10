using StylableWinFormsControls.Native;
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
        /// the width of the space between two controlbox icons
        /// </summary>
        private const int CONTROLBOX_SPACE = 5;

        /// <summary>
        /// the width of the Controlbox
        /// </summary>
        private const int CONTROLBOX_WIDTH = 32;

        private Rectangle? _closeBox;
        private Rectangle? _maximizeBox;
        private Rectangle? _minimizeBox;
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
            this.BackColor = Color.DarkGray;
            this.DoubleBuffered = true;
        }

        public Color BorderColor { get; set; } = Color.FromArgb(32, 32, 32);

        /// <summary>
        /// the Icon to close the form
        /// </summary>
        public Icon CloseIcon { get; set; } = Properties.Resources.close_window_24;

        /// <summary>
        /// the Icon to maximize the form
        /// </summary>
        public Icon MaximizeIcon { get; set; } = Properties.Resources.maximize_window_24;

        /// <summary>
        /// the Icon to minimize the form
        /// </summary>
        public Icon MinimizeIcon { get; set; } = Properties.Resources.minimize_window_24;

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
            bool runCustomWndProc = true;

            if (IsMdiChild && WindowState == FormWindowState.Maximized)
            {
                //no need to draw a titlebar or a border so we can rely on the normal Drawing
                runCustomWndProc = false;
            }
            if (!IsMdiChild)
            {
                //when the form is not an MDI child, the form will be drawn by the Desktop Window Manager
                //so we don't need to draw it ourselves as the DWM correctly styles the form
                runCustomWndProc = false;
            }

            if (
                (
                    //we want to custom draw the non-client area
                    m.Msg != (int)WindowsMessages.WM_NCPAINT &&
                    m.Msg != (int)WindowsMessages.WM_NCACTIVATE &&
                    //WM_NCLBUTTONDOWN is required to handle clicking the controlbox
                    m.Msg != (int)WindowsMessages.WM_NCLBUTTONDOWN &&
                    m.Msg != (int)WindowsMessages.WM_NCLBUTTONDBLCLK
                ) ||
                runCustomWndProc == false
                )
            {
                base.WndProc(ref m);
            }
            //custom draw the non-client area
            if (
                (
                    m.Msg == (int)WindowsMessages.WM_NCPAINT ||
                    m.Msg == (int)WindowsMessages.WM_NCACTIVATE
                ) &&
                runCustomWndProc
            )
            {
                DrawFrame();
            }
            //handle controlbox clicks
            if (
                (
                    m.Msg == (int)WindowsMessages.WM_NCLBUTTONDOWN ||
                    m.Msg == (int)WindowsMessages.WM_NCLBUTTONDBLCLK
                ) &&
                runCustomWndProc
            )
            {
                RunMouseHitCalculation(m.Msg == (int)WindowsMessages.WM_NCLBUTTONDBLCLK);
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
                controlBoxWidth = CONTROLBOX_WIDTH + CONTROLBOX_SPACE + this.GetBorderWidth();
                if (MinimizeBox)
                {
                    controlBoxWidth += CONTROLBOX_WIDTH + CONTROLBOX_SPACE;
                }
                if (MaximizeBox)
                {
                    controlBoxWidth += CONTROLBOX_WIDTH + CONTROLBOX_SPACE;
                }
            }
            //draw title bar background
            g.FillRectangle(_titleBrush, 0, 0, this.Width, this.GetTitleBarHeight());

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

            //draw controlbox
            if (ControlBox)
            {
                int startPos = this.Width - controlBoxWidth;
                var target = new Rectangle(startPos, 6, CONTROLBOX_WIDTH, this.GetTitleBarHeight() - 6 - 6);
                if (MinimizeBox)
                {
                    g.DrawIcon(MinimizeIcon, target);
                    startPos += CONTROLBOX_WIDTH + CONTROLBOX_SPACE;
                    _minimizeBox = target;
                    target = new Rectangle(startPos, 6, CONTROLBOX_WIDTH, this.GetTitleBarHeight() - 6 - 6);
                }
                if (MaximizeBox)
                {
                    g.DrawIcon(MaximizeIcon, target);
                    startPos += CONTROLBOX_WIDTH + CONTROLBOX_SPACE;
                    _maximizeBox = target;
                    target = new Rectangle(startPos, 6, CONTROLBOX_WIDTH, this.GetTitleBarHeight() - 6 - 6);
                }
                g.DrawIcon(CloseIcon, target);
                _closeBox = target;
            }
        }

        /// <summary>
        /// checks if the user clicked on a button in the controlbox
        /// </summary>
        private void RunMouseHitCalculation(bool doubleClick)
        {
            if (!ControlBox) return;
            var screenPos = Control.MousePosition;
            var clientPos = this.PointToClient(screenPos);
            //as the clientPos has the coordinates for the client area and the controlbox is not part of the client area
            //we need to update the coordinates with titlebar height and border width
            var titleBarPos = new Point(clientPos.X + this.GetBorderWidth(), clientPos.Y + this.GetTitleBarHeight());
            if (this._closeBox != null && this._closeBox.Value.Contains(titleBarPos))
            {
                this.Close();
            }
            else if (this._minimizeBox != null && this._minimizeBox.Value.Contains(titleBarPos))
            {
                this.WindowState = FormWindowState.Minimized;
            }
            else if (this._maximizeBox != null && this._maximizeBox.Value.Contains(titleBarPos))
            {
                if (this.WindowState == FormWindowState.Maximized)
                {
                    this.WindowState = FormWindowState.Normal;
                }
                else
                {
                    this.WindowState = FormWindowState.Maximized;
                }
            }
            else
            {
                //if the user double clicked the titlebar, we want to maximize the window
                if (doubleClick && this.WindowState == FormWindowState.Normal)
                {
                    this.WindowState = FormWindowState.Maximized;
                }
                //or restore the minimized window
                else if (doubleClick && this.WindowState == FormWindowState.Minimized)
                {
                    this.WindowState = FormWindowState.Normal;
                }
            }
        }
    }
}