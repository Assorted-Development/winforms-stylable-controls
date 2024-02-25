using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using StylableWinFormsControls.Extensions;

namespace StylableWinFormsControls
{
    /// <summary>
    /// A stylable base version of informational boxes with interaction possibilities
    /// </summary>
    /// <seealso cref="StylableMessageBox"/>
    public abstract class StylableInteractionBox<T> : Form where T : Control
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
        /// resource manager used to access localized texts
        /// </summary>
        private static ResourceManager _resources = new("StylableWinFormsControls.StylableInteractionBox", typeof(StylableInteractionBox<>).Assembly);

        /// <summary>
        /// contains the stylable controls for easier access than iterating over Controls
        /// </summary>
        public InteractionBoxControls<T> StylableControls { get; }

        /// <summary>
        /// constructor. not available to others as they should use the <see cref="StylableMessageBoxBuilder"/>
        /// </summary>
        /// <param name="caption">the caption</param>
        /// <param name="icon">the icon in the title bar</param>
        /// <param name="text">the messagebox text</param>
        /// <param name="buttons">describes which buttons should be shown to the user</param>
        /// <param name="defaultButton">defines which button should be selected by default</param>
        /// <param name="helpUri">the url to open when the user clicks on the help button</param>
        /// <param name="timeout">defines the intervall after which the messagebox is closed automatically</param>
        /// <param name="timeoutResult">defines the <see cref="DialogResult"/> to return when the timeout hits</param>
        internal StylableInteractionBox(
            string caption,
            MessageBoxIcon icon,
            string text,
            MessageBoxButtons buttons,
            MessageBoxDefaultButton defaultButton,
            Uri? helpUri,
            TimeSpan? timeout,
            DialogResult timeoutResult)
        {
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MinimizeBox = false;
            MaximizeBox = false;
            handleTitle(caption, icon, helpUri);
            StylableControls = new InteractionBoxControls<T>()
            {
                Text = handleText(text),
                Buttons = handleButtons(buttons, defaultButton)
            };
            OnAfterSetStylableControls();
            handleTimeouts(timeout, timeoutResult);
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

                Point currentContentPos = new(6 + marginLeft, 14 + marginTop);

                if (StylableControls.Text is not null)
                {
                    setMargin(StylableControls.Text, marginLeft, marginTop);

                    StylableControls.Text.Left = currentContentPos.X;
                    StylableControls.Text.Top = currentContentPos.Y;
                }

                currentContentPos = OnUpdateControlSizeMid(marginLeft, marginTop, currentContentPos);

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
        /// Gets called within <see cref="UpdateSize(bool)"/> between the text area and the bottom area.<br/>
        /// Can be used to add own elements to the control.
        /// </summary>
        /// <param name="marginLeft">Currently calculated highest Margin.Left value of all controls</param>
        /// <param name="marginTop">Currently calculated highest Margin.Top value of all controls</param>
        /// <param name="currentContentPos">Position at which the calculation of elements currently is.</param>
        /// <returns>Returns the position the calculation of further elements should continue</returns>
        protected virtual Point OnUpdateControlSizeMid(int marginLeft, int marginTop, Point currentContentPos)
        {
            return currentContentPos;
        }

        /// <summary>
        /// Gets called after the <see cref="StylableControls"/> object has been initialized.
        /// </summary>
        protected virtual void OnAfterSetStylableControls()
        {

        }

        /// <summary>
        /// creates the message box content
        /// </summary>
        /// <param name="text">the messagebox text</param>
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
        /// creates the required dialogResult buttons
        /// </summary>
        /// <param name="buttons">describes which buttons should be shown to the user</param>
        /// <param name="defaultButton">defines which button should be selected by default</param>
        private ReadOnlyCollection<StylableButton> handleButtons(MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton)
        {
            List<StylableButton> result = new();
            foreach (DialogResult dr in buttons.ToDialogResult())
            {
                result.Add(createButton(dr));
            }

            AcceptButton = defaultButton switch
            {
                MessageBoxDefaultButton.Button1 => result[0],
                MessageBoxDefaultButton.Button2 => result.Count > 1 ? result[1] : result.Last(),
                MessageBoxDefaultButton.Button3 => result.Count > 2 ? result[2] : result.Last(),
                MessageBoxDefaultButton.Button4 => result.Count > 3 ? result[3] : result.Last(),
                _ => result[0],
            };

            return result.AsReadOnly();
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
        /// the timer to close the messageBox
        /// </summary>
        private System.Windows.Forms.Timer? _timeout;
        /// <summary>
        /// configures timeout and timers if needed
        /// </summary>
        /// <param name="timeout">defines the intervall after which the messagebox is closed automatically</param>
        /// <param name="timeoutResult">defines the <see cref="DialogResult"/> to return when the timeout hits</param>
        private void handleTimeouts(TimeSpan? timeout, DialogResult timeoutResult)
        {
            if (timeout is not null)
            {
                _uiUpdate = new System.Windows.Forms.Timer()
                {
                    Interval = 1000
                };

                // the timeoutResult may not be necessarily in the list of available buttons
                Button defaultButton =
                    StylableControls.Buttons.FirstOrDefault(b => b.DialogResult == timeoutResult)
                        ?? StylableControls.Buttons.First(b => b == AcceptButton);

                string basicText = defaultButton.Text;

                _uiUpdate.Tick += (sender, e) =>
                {
                    _timeLeft--;
                    defaultButton!.Text = $"{basicText} ({_timeLeft}s)";
                    UpdateSize(false);
                };

                _timeout = new System.Windows.Forms.Timer()
                {
                    Interval = (int)timeout.Value.TotalMilliseconds
                };
                _timeout.Tick += (sender, e) =>
                {
                    DialogResult = timeoutResult;
                    Close();
                };

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
