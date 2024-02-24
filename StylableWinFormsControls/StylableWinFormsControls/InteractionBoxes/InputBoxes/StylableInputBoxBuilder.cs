namespace StylableWinFormsControls
{
    /// <summary>
    /// the builder to configure the <see cref="StylableInputBox"/>
    /// </summary>
    public sealed class StylableInputBoxBuilder : StylableInteractionBoxBuilder
    {
        /// <summary>
        /// creates the <see cref="StylableInputBox"/> for text input
        /// </summary>
        /// <param name="startValue">the value to show at the start</param>
        /// <returns>the completely configured but unstyled <see cref="StylableInputBox{TextBox}"/></returns>
        public StylableInputBox<TextBox> ForText(string startValue = "")
        {
            TextBox textBox = new()
            {
                Text = startValue
            };
            return new StylableInputBox<TextBox>(Caption, Icon, Text, Buttons, DefaultButton, HelpUri, Timeout, TimeoutResult, textBox);
        }

        /// <summary>
        /// creates the <see cref="StylableInputBox"/> for text input
        /// </summary>
        /// <returns>the completely configured but unstyled <see cref="StylableInputBox{NumericUpDown}"/></returns>
        public StylableInputBox<NumericUpDown> ForNumericValue(decimal startValue = 0, decimal minValue = decimal.MinValue, decimal maxValue = decimal.MaxValue)
        {
            NumericUpDown nup = new()
            {
                Minimum = minValue,
                Maximum = maxValue,
                Value = startValue
            };
            return new StylableInputBox<NumericUpDown>(Caption, Icon, Text, Buttons, DefaultButton, HelpUri, Timeout, TimeoutResult, nup);
        }


        public StylableInputBoxBuilder WithButtons(
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
        public StylableInputBoxBuilder WithText(string text)
        {
            SetText(text);
            return this;
        }
        /// <summary>
        /// Configure the title bar
        /// </summary>
        /// <param name="caption">the caption</param>
        /// <param name="icon">the icon</param>
        public StylableInputBoxBuilder WithTitle(string caption = "", MessageBoxIcon icon = MessageBoxIcon.None)
        {
            SetTitle(caption, icon);
            return this;
        }
        /// <summary>
        /// when called, the dialog will close after the given timeout and return the given default result
        /// </summary>
        /// <param name="timeout">defines the intervall after which the messagebox is closed automatically</param>
        /// <param name="timeoutResult">defines the <see cref="DialogResult"/> to return when the timeout hits</param>
        public StylableInputBoxBuilder WithTimeout(TimeSpan timeout, DialogResult timeoutResult = DialogResult.Cancel)
        {
            SetTimeout(timeout, timeoutResult);
            return this;
        }
        /// <summary>
        /// Shows the help button in the title bar
        /// </summary>
        /// <param name="url">the url to open when the user clicks on the help button</param>
        /// <exception cref="ArgumentNullException">url must be non-null</exception>
        public StylableInputBoxBuilder WithHelpButton(string url)
        {
            SetHelpButton(url);
            return this;
        }
        /// <summary>
        /// Shows the help button in the title bar
        /// </summary>
        /// <param name="uri">the url to open when the user clicks on the help button</param>
        /// <exception cref="ArgumentNullException">url must be non-null</exception>
        public StylableInputBoxBuilder WithHelpButton(Uri uri)
        {
            SetHelpButton(uri);
            return this;
        }
    }
}
