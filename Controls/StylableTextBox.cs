using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MFBot_1701_E.Theming;

namespace AssortedDevelopment.StylableWinFormsControls
{
    public class StylableTextBox : TextBox
    {
        private Timer m_delayedTextChangedTimer;
        private string _hint;

        public event EventHandler DelayedTextChanged;

        private Color borderColor = Color.Blue;
        public Color BorderColor
        {
            get => borderColor;
            set
            {
                borderColor = value;
            }
        }
        [Editor]
        public int DelayedTextChangedTimeout { get; set; }

        [Editor]
        [Description("If set, this text will be shown as grayed hint until the user enters the box")]
        public string Hint
        {
            get => _hint;
            set
            {
                _hint = value;
                if (string.IsNullOrEmpty(base.Text))
                    base.Text = value;
            }
        }

        public bool IsHintActive { get; private set; }

        public bool IsDelayActive { get; set; } = true;

        public new string Text => base.Text == Hint ? "" : base.Text;

        public StylableTextBox()
        {
            DelayedTextChangedTimeout = 900; // 0.9 seconds

            /* standard on initializing form: hint is enabled */
            ThemeRegistry.Current.Apply(this, ThemeOptions.Hint);
            IsHintActive = true;
            BorderStyle = BorderStyle.None;
        }

        protected override void Dispose(bool disposing)
        {
            if (m_delayedTextChangedTimer != null)
            {
                m_delayedTextChangedTimer.Stop();
                if (disposing)
                    m_delayedTextChangedTimer.Dispose();
            }

            base.Dispose(disposing);
        }
        

        #region timer events / methods
        protected virtual void OnDelayedTextChanged(EventArgs e)
        {
            DelayedTextChanged?.Invoke(this, e);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            if (IsDelayActive && DelayedTextChangedTimeout > 0)
            {
                InitializeDelayedTextChangedEvent();
            }
            base.OnTextChanged(e);
        }

        private void InitializeDelayedTextChangedEvent()
        {
            m_delayedTextChangedTimer?.Stop();

            if (m_delayedTextChangedTimer == null || m_delayedTextChangedTimer.Interval != DelayedTextChangedTimeout)
            {
                m_delayedTextChangedTimer = new Timer();
                m_delayedTextChangedTimer.Tick += HandleDelayedTextChangedTimerTick;
                m_delayedTextChangedTimer.Interval = DelayedTextChangedTimeout;
            }

            m_delayedTextChangedTimer.Start();
        }

        private void HandleDelayedTextChangedTimerTick(object sender, EventArgs e)
        {
            if (sender is Timer timer)
            {
                timer.Stop();
            }

            OnDelayedTextChanged(EventArgs.Empty);
        }
        #endregion
        #region hint events / methods

        protected override void OnLostFocus(EventArgs e)
        {
            if (!IsHintActive && string.IsNullOrEmpty(base.Text))
            {
                IsHintActive = true;
                ThemeRegistry.Current.Apply(this, ThemeOptions.Hint);
                base.Text = Hint;
            }

            base.OnLostFocus(e);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            if (IsHintActive && !string.IsNullOrEmpty(base.Text))
            {
                IsHintActive = false;
                base.Text = "";
                ThemeRegistry.Current.Apply(this, ThemeOptions.None);
            }

            base.OnGotFocus(e);
        }
        #endregion
    }
}
