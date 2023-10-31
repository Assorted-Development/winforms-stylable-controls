using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;

using StylableWinFormsControls.Extensions;

namespace StylableWinFormsControls
{
    /// <summary>
    /// A stylable version of the <see cref="MessageBox"/>
    /// </summary>
    public sealed class StylableMessageBox : Form
    {
        /// <summary>
        /// additional form width
        /// </summary>
        public const int BORDER_WIDTH = 40;
        /// <summary>
        /// additional form height
        /// </summary>
        public const int BORDER_HEIGHT = 70;
        /// <summary>
        /// returns a builder object to configure the <see cref="StylableMessageBox"/>
        /// </summary>
        public static StylableMessageBoxBuilder BUILDER => new();
        /// <summary>
        /// resource manager used to access localized texts
        /// </summary>
        private static ResourceManager _resources = new(typeof(StylableMessageBox));

        /// <summary>
        /// the checkstate of the checkbox if shown
        /// </summary>
        public CheckState CheckState { get; private set; } = CheckState.Indeterminate;
        /// <summary>
        /// contains the stylable controls for easier access than iterating over Controls
        /// </summary>
        public MessageBoxControls StylableControls { get; }
        /// <summary>
        /// constructor. not available to others as they should use the <see cref="StylableMessageBoxBuilder"/>
        /// </summary>
        /// <param name="caption">the caption</param>
        /// <param name="icon">the icon in the title bar</param>
        /// <param name="text">the messagebox text</param>
        /// <param name="buttons">describes which buttons should be shown to the user</param>
        /// <param name="defaultButton">defines which button should be selected by default</param>
        /// <param name="helpUri">the url to open when the user clicks on the help button</param>
        /// <param name="checkboxText">the checkbox text to be shown to the user</param>
        /// <param name="timeout">defines the intervall after which the messagebox is closed automatically</param>
        /// <param name="timeoutResult">defines the <see cref="DialogResult"/> to return when the timeout hits</param>
        internal StylableMessageBox(string caption, MessageBoxIcon icon, string text, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, Uri? helpUri, string? checkboxText, TimeSpan? timeout, DialogResult timeoutResult)
        {
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MinimizeBox = false;
            MaximizeBox = false;
            handleTitle(caption, icon, helpUri);
            StylableControls = new MessageBoxControls()
            {
                Text = handleText(text),
                CheckBox = handleCheckBox(checkboxText),
                Buttons = handleButtons(buttons, defaultButton)
            };
            handleTimeouts(timeout, timeoutResult);
            UpdateSize();
        }
        /// <summary>
        /// resize the form to fit the content
        /// </summary>
        /// <param name="updateControlSize">if true, the sizes and positions of all controls will be recalculated. otherwise, only the form will be updated</param>
        public void UpdateSize(bool updateControlSize = true)
        {
            if (updateControlSize)
            {
                Point currentContentPos = new(9, 20);
                StylableControls.Text.Left = currentContentPos.X;
                StylableControls.Text.Top = currentContentPos.Y;
                currentContentPos.Y = currentContentPos.Y + StylableControls.Text.Height + 10;
                if (StylableControls.CheckBox is not null)
                {
                    StylableControls.CheckBox.Left = currentContentPos.X;
                    StylableControls.CheckBox.Top = currentContentPos.Y;
                    currentContentPos.Y = currentContentPos.Y + StylableControls.CheckBox.Height + 10;
                }

                foreach (StylableButton sb in StylableControls.Buttons)
                {
                    sb.Left = currentContentPos.X;
                    sb.Top = currentContentPos.Y;
                    currentContentPos.X = currentContentPos.X + sb.Width + 10;
                }
            }
            Width = (from c in Controls.Cast<Control>() select c.Left + c.Width + BORDER_WIDTH).Max();
            Height = (from c in Controls.Cast<Control>() select c.Top + c.Height + BORDER_HEIGHT).Max();
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
        /// creates the message box content
        /// </summary>
        /// <param name="checkboxText">the checkbox text to be shown to the user</param>
        private StylableCheckBox? handleCheckBox(string? checkboxText)
        {
            if (checkboxText is null)
            {
                return null;
            }
            StylableCheckBox checkbox = new()
            {
                Text = checkboxText,
                AutoSize = true,
            };
            checkbox.CheckStateChanged += (sender, e) => CheckState = checkbox.CheckState;

            Controls.Add(checkbox);
            return checkbox;
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
