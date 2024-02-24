using System.Data;
using System.Diagnostics;
using System.Globalization;
using StylableWinFormsControls.InputBoxes;
using System.Resources;

namespace StylableWinFormsControls
{
    public class StylableInputBox<T> : Form where T : Control
    {
        /// <summary>
        /// additional form width
        /// </summary>
        public const int BORDER_WIDTH = 20;
        /// <summary>
        /// additional form height
        /// </summary>
        public const int BORDER_HEIGHT = 10;
        /// <summary>
        /// returns a builder object to configure the <see cref="StylableInputBox"/>
        /// </summary>
        public static StylableInputBoxBuilder BUILDER => new();
        /// <summary>
        /// resource manager used to access localized texts (does not use constructor with type as this fails with generic types)
        /// </summary>
        private static ResourceManager _resources = new("StylableWinFormsControls.StylableInputBox", typeof(StylableInputBox).Assembly);
        /// <summary>
        /// contains the stylable controls for easier access than iterating over Controls
        /// </summary>
        public InputBoxControls<T> StylableControls { get; }
        /// <summary>
        /// constructor. not available to others as they should use the <see cref="StylableMessageBoxBuilder"/>
        /// </summary>
        /// <param name="caption">the caption</param>
        /// <param name="icon">the icon in the title bar</param>
        /// <param name="text">the prompt text</param>
        /// <param name="defaultButton">defines which button should be selected by default</param>
        /// <param name="helpUri">the url to open when the user clicks on the help button</param>
        /// <param name="timeout">defines the intervall after which the messagebox is closed automatically</param>
        /// <param name="timeoutResult">defines the <see cref="DialogResult"/> to return when the timeout hits</param>
        /// <param name="inputControl">the control to input the value</param>
        internal StylableInputBox(string caption, MessageBoxIcon icon, string text, MessageBoxDefaultButton defaultButton, Uri? helpUri, TimeSpan? timeout, DialogResult timeoutResult, T inputControl)
        {
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MinimizeBox = false;
            MaximizeBox = false;
            handleTitle(caption, icon, helpUri);
            StylableControls = new InputBoxControls<T>(
                handleText(text),
                createButton(DialogResult.OK),
                createButton(DialogResult.Cancel),
                handleInput(inputControl)
            );
            handleTimeouts(timeout, timeoutResult);
            UpdateSize();
        }
        /// <summary>
        /// resize the form to fit the content
        /// </summary>
        /// <param name="updateControlSize">if true, the sizes and positions of all controls will be recalculated. otherwise, only the form will be updated</param>
        public void UpdateSize(bool updateControlSize = true)
        {
            int titleBarHeight = getWindowTitleBarHeight();

            if (updateControlSize)
            {
                int marginLeft = (from c in Controls.Cast<Control>() select c.Margin.Left).Min();
                int marginTop = (from c in Controls.Cast<Control>() select c.Margin.Top).Min();

                Point currentContentPos = new(6 + marginLeft, 20 + marginTop);

                if (StylableControls.Text is not null)
                {
                    setMargin(StylableControls.Text, marginLeft, marginTop);

                    StylableControls.Text.Left = currentContentPos.X;
                    StylableControls.Text.Top = currentContentPos.Y;
                }

                currentContentPos.Y += StylableControls.Text is null ? 0 : StylableControls.Text.Height + 6;

                // Margins on CheckBoxes seem to not work directly
                StylableControls.InputControl.Left = currentContentPos.X + marginLeft;
                StylableControls.InputControl.Top = currentContentPos.Y + marginTop;
                currentContentPos.Y += StylableControls.InputControl.Height + 6;

                currentContentPos.Y += 16;

                foreach (StylableButton sb in StylableControls.Buttons)
                {
                    setMargin(sb, marginLeft, marginTop);
                    sb.Left = currentContentPos.X;
                    sb.Top = currentContentPos.Y;

                    currentContentPos.X = currentContentPos.X + sb.Width + 10;
                }
            }

            Width = (from c in Controls.Cast<Control>() select c.Left + c.Width + c.Margin.Left + c.Margin.Right + BORDER_WIDTH).Max();
            Height = (from c in Controls.Cast<Control>() select c.Top + c.Height + c.Margin.Top + c.Margin.Bottom + BORDER_HEIGHT + titleBarHeight).Max();
        }
        /// <summary>
        /// creates the prompt text
        /// </summary>
        /// <param name="text">the prompt text</param>
        private StylableLabel handleText(string text)
        {
            StylableLabel label = new()
            {
                Text = text,
                AutoSize = true
            };
            Controls.Add(label);
            return label;
        }
        /// <summary>
        /// adds the input control
        /// </summary>
        /// <param name="input">the input control</param>
        private T handleInput(T input)
        {
            Controls.Add(input);
            return input;
        }
        /// <summary>
        /// Create a button for the given DialogResult
        /// </summary>
        /// <param name="result"></param>
        private StylableButton createButton(DialogResult result)
        {
            StylableButton b = new()
            {
                Text = _resources.GetString(result.ToString(), CultureInfo.CurrentCulture),
                DialogResult = result,
                AutoSize = true
            };
            Controls.Add(b);
            return b;
        }
        /// <summary>
        /// the time left before the messageBox closes automatically
        /// </summary>
        private int _timeLeft;
        /// <summary>
        /// time for updating the time left on the default button
        /// </summary>
        private System.Windows.Forms.Timer? _uiUpdate;
        /// <summary>
        /// the timer to close the inputbox
        /// </summary>
        private System.Windows.Forms.Timer? _timeout;
        /// <summary>
        /// configures timeout and timers if needed
        /// </summary>
        /// <param name="timeout">defines the intervall after which the inputbox is closed automatically</param>
        /// <param name="timeoutResult">defines the <see cref="DialogResult"/> to return when the timeout hits</param>
        private void handleTimeouts(TimeSpan? timeout, DialogResult timeoutResult)
        {
            if (timeout is not null)
            {
                _uiUpdate = new System.Windows.Forms.Timer()
                {
                    Interval = 1000
                };
                //the timeoutResult may not be necessarily in the list of available buttons
                Button defaultButton = StylableControls.Buttons.FirstOrDefault(b => b.DialogResult == timeoutResult) ?? StylableControls.Buttons.First(b => b == AcceptButton);
                string basicText = defaultButton.Text;
                _uiUpdate.Tick += (sender, e) => { _timeLeft--; defaultButton!.Text = $"{basicText} ({_timeLeft}s)"; };

                _timeout = new System.Windows.Forms.Timer()
                {
                    Interval = (int)timeout.Value.TotalMilliseconds
                };
                _timeout.Tick += (sender, e) => { DialogResult = timeoutResult; Close(); };

                VisibleChanged += (sender, e) =>
                {
                    if (Visible)
                    {
                        _timeLeft = (int)timeout.Value.TotalSeconds;
                        _uiUpdate.Start();
                        _timeout.Start();
                    }
                    else
                    {
                        _uiUpdate.Stop();
                        _timeout.Stop();
                    }
                };
            }
        }
        /// <summary>
        /// configures the title bar
        /// </summary>
        /// <param name="caption">the caption</param>
        /// <param name="icon">the icon in the title bar</param>
        /// <param name="helpUri">the url to open when the user clicks on the help button</param>
        private void handleTitle(string caption, MessageBoxIcon icon, Uri? helpUri)
        {
            Text = caption;
            Icon = icon switch
            {
                MessageBoxIcon.Error => SystemIcons.Error,
                MessageBoxIcon.Exclamation => SystemIcons.Exclamation,
                MessageBoxIcon.Information => SystemIcons.Information,
                MessageBoxIcon.Question => SystemIcons.Question,
                _ => null
            };
            if (helpUri is not null)
            {
                HelpButton = true;
                HelpButtonClicked += (sender, e) => Process.Start(new ProcessStartInfo(helpUri.ToString()) { UseShellExecute = true });
            }
        }
        private int getWindowTitleBarHeight()
        {
            Rectangle screenRectangle = RectangleToScreen(ClientRectangle);
            return screenRectangle.Top - Top;
        }

        private static void setMargin(Control? c, int marginLeft, int marginTop)
        {
            if (c is null)
            {
                return;
            }

            c.Margin = new Padding(marginLeft, marginTop, c.Margin.Right, c.Margin.Top);
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _uiUpdate?.Dispose();
                _timeout?.Dispose();
            }
        }
    }
}
