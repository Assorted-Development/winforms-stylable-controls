namespace StylableWinFormsControls
{
    /// <summary>
    /// A stylable version of the <see cref="MessageBox"/>
    /// </summary>
    public sealed class StylableMessageBox : StylableInteractionBox<CheckBox>
    {
        /// <summary>
        /// returns a builder object to configure the <see cref="StylableMessageBox"/>
        /// </summary>
        public static StylableMessageBoxBuilder BUILDER => new();

        /// <summary>
        /// the checkstate of the checkbox if shown
        /// </summary>
        public CheckState CheckState { get; private set; } = CheckState.Indeterminate;

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
        internal StylableMessageBox(
            string caption,
            MessageBoxIcon icon,
            string text,
            MessageBoxButtons buttons,
            MessageBoxDefaultButton defaultButton,
            Uri? helpUri,
            string? checkboxText,
            TimeSpan? timeout,
            DialogResult timeoutResult
        ) : base(caption, icon, text, buttons, defaultButton, helpUri, timeout, timeoutResult)
        {
            StylableControls.InputControl = handleCheckBox(checkboxText);
            UpdateSize();
        }

        protected override Point OnUpdateControlSizeMid(int marginLeft, int marginTop, Point currentContentPos)
        {
            currentContentPos.Y += StylableControls.Text is null ? 0 : StylableControls.Text.Height + 6;
            if (StylableControls.InputControl is not null)
            {
                // Margins on CheckBoxes seem to not work directly
                StylableControls.InputControl.Left = currentContentPos.X + marginLeft;
                StylableControls.InputControl.Top = currentContentPos.Y + marginTop;
                currentContentPos.Y += StylableControls.InputControl.Height + 6;
            }

            currentContentPos.Y += 16;
            return currentContentPos;
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
    }
}
