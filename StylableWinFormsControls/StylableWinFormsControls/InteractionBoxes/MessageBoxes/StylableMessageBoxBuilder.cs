namespace StylableWinFormsControls
{
    /// <summary>
    /// the builder to configure the <see cref="StylableInteractionBox"/>
    /// </summary>
    public sealed class StylableMessageBoxBuilder : StylableInteractionBoxBuilder<StylableMessageBoxBuilder>
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
