namespace StylableWinFormsControls
{
    /// <summary>
    /// the builder to configure the <see cref="StylableInteractionBox"/>
    /// </summary>
    public sealed class StylableMessageBoxBuilder : StylableInteractionBoxBuilder
    {
        /// <summary>
        /// the checkbox text to be shown to the user
        /// </summary>
        private string? _checkboxText;

        /// <summary>
        /// creates the <see cref="StylableMessageBox"/>
        /// </summary>
        /// <returns>the completely configured but unstyled <see cref="StylableMessageBox"/></returns>
        public StylableMessageBox Build()
        {
            return new StylableMessageBox(Caption, Icon, Text, Buttons, DefaultButton,
                HelpUri, _checkboxText, Timeout, TimeoutResult);
        }

        public StylableMessageBoxBuilder WithButtons(
            MessageBoxButtons buttons,
            MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
        {
            SetButtons(buttons, defaultButton);
            return this;
        }
        /// <summary>
        /// Set the messagebox text
        /// </summary>
        /// <param name="text"></param>
        public StylableMessageBoxBuilder WithText(string text)
        {
            SetText(text);
            return this;
        }
        /// <summary>
        /// Configure the title bar
        /// </summary>
        /// <param name="caption">the caption</param>
        /// <param name="icon">the icon</param>
        public StylableMessageBoxBuilder WithTitle(string caption = "", MessageBoxIcon icon = MessageBoxIcon.None)
        {
            SetTitle(caption, icon);
            return this;
        }
        /// <summary>
        /// when called, the dialog will close after the given timeout and return the given default result
        /// </summary>
        /// <param name="timeout">defines the intervall after which the messagebox is closed automatically</param>
        /// <param name="timeoutResult">defines the <see cref="DialogResult"/> to return when the timeout hits</param>
        public StylableMessageBoxBuilder WithTimeout(TimeSpan timeout, DialogResult timeoutResult = DialogResult.Cancel)
        {
            SetTimeout(timeout, timeoutResult);
            return this;
        }
        /// <summary>
        /// Shows the help button in the title bar
        /// </summary>
        /// <param name="url">the url to open when the user clicks on the help button</param>
        /// <exception cref="ArgumentNullException">url must be non-null</exception>
        public StylableMessageBoxBuilder WithHelpButton(string url)
        {
            SetHelpButton(url);
            return this;
        }
        /// <summary>
        /// Shows the help button in the title bar
        /// </summary>
        /// <param name="uri">the url to open when the user clicks on the help button</param>
        /// <exception cref="ArgumentNullException">url must be non-null</exception>
        public StylableMessageBoxBuilder WithHelpButton(Uri uri)
        {
            SetHelpButton(uri);
            return this;
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
    }
}
