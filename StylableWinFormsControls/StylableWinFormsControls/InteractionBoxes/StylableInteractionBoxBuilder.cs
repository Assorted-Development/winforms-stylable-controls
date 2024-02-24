namespace StylableWinFormsControls
{
    /// <summary>
    /// the builder to configure all informational boxes Set interaction possibilities
    /// </summary>
    /// <seealso cref="StylableInteractionBox"/>
    public abstract class StylableInteractionBoxBuilder
    {
        /// <summary>
        /// the caption
        /// </summary>
        internal string Caption = string.Empty;
        /// <summary>
        /// the icon in the title bar
        /// </summary>
        internal MessageBoxIcon Icon = MessageBoxIcon.None;
        /// <summary>
        /// the messagebox text
        /// </summary>
        internal string Text = string.Empty;
        /// <summary>
        /// describes which buttons should be shown to the user
        /// </summary>
        internal MessageBoxButtons Buttons = MessageBoxButtons.OK;
        /// <summary>
        /// defines which button should be selected by default
        /// </summary>
        internal MessageBoxDefaultButton DefaultButton = MessageBoxDefaultButton.Button1;
        /// <summary>
        /// the url to open when the user clicks on the help button
        /// </summary>
        internal Uri? HelpUri;
        /// <summary>
        /// defines the intervall after which the messagebox is closed automatically
        /// </summary>
        internal TimeSpan? Timeout;
        /// <summary>
        /// defines the <see cref="DialogResult"/> to return when the timeout hits
        /// </summary>
        internal DialogResult TimeoutResult = DialogResult.None;

        /// <summary>
        /// configures which buttons to show and which button should be the default button
        /// </summary>
        /// <param name="buttons">describes which buttons should be shown to the user</param>
        /// <param name="defaultButton">defines which button should be selected by default</param>
        protected void SetButtons(
            MessageBoxButtons buttons,
            MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
        {
            Buttons = buttons;
            DefaultButton = defaultButton;
        }
        /// <summary>
        /// Set the messagebox text
        /// </summary>
        /// <param name="text"></param>
        protected void SetText(string text)
        {
            Text = text ?? string.Empty;
        }
        /// <summary>
        /// Configure the title bar
        /// </summary>
        /// <param name="caption">the caption</param>
        /// <param name="icon">the icon</param>
        protected void SetTitle(string caption = "", MessageBoxIcon icon = MessageBoxIcon.None)
        {
            Caption = caption ?? string.Empty;
            Icon = icon;
        }
        /// <summary>
        /// when called, the dialog will close after the given timeout and return the given default result
        /// </summary>
        /// <param name="timeout">defines the intervall after which the messagebox is closed automatically</param>
        /// <param name="timeoutResult">defines the <see cref="DialogResult"/> to return when the timeout hits</param>
        protected void SetTimeout(TimeSpan timeout, DialogResult timeoutResult = DialogResult.Cancel)
        {
            Timeout = timeout;
            TimeoutResult = timeoutResult;
        }
        /// <summary>
        /// Shows the help button in the title bar
        /// </summary>
        /// <param name="url">the url to open when the user clicks on the help button</param>
        /// <exception cref="ArgumentNullException">url must be non-null</exception>
        protected void SetHelpButton(string url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }
            SetHelpButton(new Uri(url));
        }
        /// <summary>
        /// Shows the help button in the title bar
        /// </summary>
        /// <param name="url">the url to open when the user clicks on the help button</param>
        /// <exception cref="ArgumentNullException">url must be non-null</exception>
        protected void SetHelpButton(Uri url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }
            HelpUri = url;
        }
    }
}
