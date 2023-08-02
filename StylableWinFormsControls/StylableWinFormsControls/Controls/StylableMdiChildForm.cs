using System.Runtime.InteropServices;
using System.Windows.Forms.VisualStyles;

namespace StylableWinFormsControls.Controls
{
    /// <summary>
    /// the default form does not align with the windows theme when used as an MDI child
    /// </summary>
    public partial class StylableMdiChildForm : Form
    {
        /// <summary>
        /// Sent to a window when its nonclient area needs to be changed to indicate an active or
        /// inactive state.
        /// </summary>
        private const int WM_NCACTIVATE = 0x0086;

        /// <summary>
        /// The WM_NCPAINT message is sent to a window when its frame must be painted.
        /// </summary>
        private const int WM_NCPAINT = 0x0085;

        /// <summary>
        /// defines the place where the icon is drawn
        /// </summary>
        private Rectangle _titleIconCanvas = new Rectangle(6, 6, 20, 20);

        private Color _borderColor = Color.FromArgb(32,32,32);
        private Brush _borderBrush = new SolidBrush(Color.FromArgb(32, 32, 32));
        private Brush _titleBrush = new SolidBrush(Color.FromArgb(32, 32, 32));
        private Color _titleColor = Color.FromArgb(32, 32, 32);
        private int _titleHeight = 0;
        public int TitleHeight {
            get
            {
                return _titleHeight;
            }
            set
            {
                _titleHeight = value;
                _titleIconCanvas = new Rectangle(6, 6, _titleHeight - 12, _titleHeight - 12);
                RecalcDimensions();
            }
        }
        private int _borderWidth = 0;
        public int BorderWidth {
            get
            {
                return _borderWidth;
            }
            set
            {
                _borderWidth = value;
                RecalcDimensions();
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        public StylableMdiChildForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        public Color BorderColor
        {
            get
            {
                return _borderColor;
            }
            set
            {
                _borderColor = value;
                _borderBrush = new SolidBrush(_borderColor);
            }
        }

        public Color TitleColor
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


        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (IsMdiChild && WindowState == FormWindowState.Maximized)
            {
                //no need to draw a titlebar or a border so we can rely on the normal Drawing
                return;
            }
            if(!IsMdiChild)
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
                //draw title bar
                g.FillRectangle(_titleBrush, 0, 0, this.Width, TitleHeight);
                g.DrawString(this.Text, this.Font, Brushes.Coral, 35, 8);
                if (this.Icon != null)
                    g.DrawIcon(this.Icon, _titleIconCanvas);
                //draw left border
                g.FillRectangle(_borderBrush, 0, TitleHeight, BorderWidth, this.Height - TitleHeight);
                //draw bottom border
                g.FillRectangle(_borderBrush, BorderWidth, this.Height - BorderWidth, this.ClientSize.Width, BorderWidth);
                //draw right border
                g.FillRectangle(_borderBrush, this.Width - BorderWidth, TitleHeight, BorderWidth, this.Height - TitleHeight);
            }
            ReleaseDC(this.Handle, hdc);
        }

        private void RecalcDimensions()
        {
            if (TitleHeight == 0)
                TitleHeight = this.Height - this.ClientSize.Height - BorderWidth - 8;
            if (BorderWidth == 0)
                BorderWidth = (this.Width - this.ClientSize.Width) / 2;
            this.Height = _titleHeight + this.ClientSize.Height + BorderWidth + 8;
            this.Width = 2 * BorderWidth + this.ClientSize.Width;
        }

        private void Form_Load(object sender, EventArgs e)
        {
            RecalcDimensions();
        }
    }
}