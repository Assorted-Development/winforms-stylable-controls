using System.Security.Policy;

namespace StylableWinFormsControls
{
    /// <summary>
    /// the builder to configure all informational boxes Set interaction possibilities
    /// </summary>
    /// <seealso cref="StylableInteractionBox"/>
    public abstract class StylableInteractionBoxBuilder<T> where T : StylableInteractionBoxBuilder<T>
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
        public T WithButtons(
            MessageBoxButtons buttons,
            MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
        {
            Buttons = buttons;
            DefaultButton = defaultButton;
            return (T)(object)this;
        }
        /// <summary>
        /// Set the messagebox/prompt text
        /// </summary>
        /// <param name="text"></param>
        public T WithText(string text)
        {
            Text = text ?? string.Empty;
            return (T)(object)this;
        }
        /// <summary>
        /// Configure the title bar
        /// </summary>
        /// <param name="caption">the caption</param>
        /// <param name="icon">the icon</param>
        public T WithTitle(string caption = "", MessageBoxIcon icon = MessageBoxIcon.None)
        {
            Caption = caption ?? string.Empty;
            Icon = icon;
            return (T)(object)this;
        }
        /// <summary>
        /// when called, the dialog will close after the given timeout and return the given default result
        /// </summary>
        /// <param name="timeout">defines the intervall after which the messagebox is closed automatically</param>
        /// <param name="timeoutResult">defines the <see cref="DialogResult"/> to return when the timeout hits</param>
        public T WithTimeout(TimeSpan timeout, DialogResult timeoutResult = DialogResult.Cancel)
        {
            Timeout = timeout;
            TimeoutResult = timeoutResult;
            return (T)(object)this;
        }
        /// <summary>
        /// Shows the help button in the title bar
        /// </summary>
        /// <param name="url">the url to open when the user clicks on the help button</param>
        /// <exception cref="ArgumentNullException">url must be non-null</exception>
        public T WithHelpButton(string url)
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
        /// <param name="uri">the url to open when the user clicks on the help button</param>
        /// <exception cref="ArgumentNullException">url must be non-null</exception>
        public T WithHelpButton(Uri uri)
        {
            if (uri is null)
            {
                throw new ArgumentNullException(nameof(uri));
            }
            HelpUri = uri;
            return (T)(object)this;
        }
    }
}
