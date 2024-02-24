

namespace StylableWinFormsControls
{
    /// <summary>
    /// the builder to configure the <see cref="StylableMessageBox"/>
    /// </summary>
    public sealed class StylableMessageBoxBuilder
    {
        /// <summary>
        /// the caption
        /// </summary>
        private string _caption = string.Empty;
        /// <summary>
        /// the icon in the title bar
        /// </summary>
        private MessageBoxIcon _icon = MessageBoxIcon.None;
        /// <summary>
        /// the messagebox text
        /// </summary>
        private string _text = string.Empty;
        /// <summary>
        /// describes which buttons should be shown to the user
        /// </summary>
        private MessageBoxButtons _buttons = MessageBoxButtons.OK;
        /// <summary>
        /// defines which button should be selected by default
        /// </summary>
        private MessageBoxDefaultButton _defaultButton = MessageBoxDefaultButton.Button1;
        /// <summary>
        /// the url to open when the user clicks on the help button
        /// </summary>
        private Uri? _helpUri;
        /// <summary>
        /// the checkbox text to be shown to the user
        /// </summary>
        private string? _checkboxText;
        /// <summary>
        /// defines the intervall after which the messagebox is closed automatically
        /// </summary>
        private TimeSpan? _timeout;
        /// <summary>
        /// defines the <see cref="DialogResult"/> to return when the timeout hits
        /// </summary>
        private DialogResult _timeoutResult = DialogResult.None;
        /// <summary>
        /// creates the <see cref="StylableMessageBox"/>
        /// </summary>
        /// <returns>the completely configured but unstyled <see cref="StylableMessageBox"/></returns>
        public StylableMessageBox Build()
        {
            return new StylableMessageBox(_caption, _icon, _text, _buttons, _defaultButton,
                _helpUri, _checkboxText, _timeout, _timeoutResult);
        }
        /// <summary>
        /// This adds a <see cref="StylableCheckBox"/> to the form. use <see cref="CheckState"/> to get the check state
        /// </summary>
        /// <param name="checkBoxText">the text to be shown to the user</param>
        public StylableMessageBoxBuilder WithCheckBox(string checkBoxText)
        {
            _checkboxText = checkBoxText ?? string.Empty;
            return this;
        }
        /// <summary>
        /// configures which buttons to show and which button should be the default button
        /// </summary>
        /// <param name="buttons">describes which buttons should be shown to the user</param>
        /// <param name="defaultButton">defines which button should be selected by default</param>
        public StylableMessageBoxBuilder WithButtons(MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
        {
            _buttons = buttons;
            _defaultButton = defaultButton;
            return this;
        }
        /// <summary>
        /// Set the messagebox text
        /// </summary>
        /// <param name="text"></param>
        public StylableMessageBoxBuilder WithText(string text)
        {
            _text = text ?? string.Empty;
            return this;
        }
        /// <summary>
        /// Configure the title bar
        /// </summary>
        /// <param name="caption">the caption</param>
        /// <param name="icon">the icon</param>
        public StylableMessageBoxBuilder WithTitle(string caption = "", MessageBoxIcon icon = MessageBoxIcon.None)
        {
            _caption = caption ?? string.Empty;
            _icon = icon;
            return this;
        }
        /// <summary>
        /// when called, the dialog will close after the given timeout and return the given default result
        /// </summary>
        /// <param name="timeout">defines the intervall after which the messagebox is closed automatically</param>
        /// <param name="timeoutResult">defines the <see cref="DialogResult"/> to return when the timeout hits</param>
        public StylableMessageBoxBuilder WithTimeout(TimeSpan timeout, DialogResult timeoutResult = DialogResult.Cancel)
        {
            _timeout = timeout;
            _timeoutResult = timeoutResult;
            return this;
        }
        /// <summary>
        /// Shows the help button in the title bar
        /// </summary>
        /// <param name="url">the url to open when the user clicks on the help button</param>
        /// <exception cref="ArgumentNullException">url must be non-null</exception>
        public StylableMessageBoxBuilder WithHelpButton(string url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }
            return WithHelpButton(new Uri(url));
        }
        /// <summary>
        /// Shows the help button in the title bar
        /// </summary>
        /// <param name="url">the url to open when the user clicks on the help button</param>
        /// <exception cref="ArgumentNullException">url must be non-null</exception>
        public StylableMessageBoxBuilder WithHelpButton(Uri url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }
            _helpUri = url;
            return this;
        }
    }
}
