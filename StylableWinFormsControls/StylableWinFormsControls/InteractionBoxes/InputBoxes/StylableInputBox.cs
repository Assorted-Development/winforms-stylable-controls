namespace StylableWinFormsControls
{
    /// <summary>
    /// A stylable version of  VB.NETs Interaction.InputBox
    /// </summary>
    /// <typeparam name="T">the type of the input control</typeparam>
    /// <typeparam name="U">the type of the value returned from the input control</typeparam>
    public abstract class StylableInputBox<T, U> : StylableInteractionBox<T> where T : Control
    {
        /// <summary>
        /// Returns a builder object to configure the <see cref="StylableInputBox"/>
        /// </summary>
        public static StylableInputBoxBuilder BUILDER => new();
        /// <summary>
        /// accessor to return the current value
        /// </summary>
        private Func<T, U> _accessor;

        /// <summary>
        /// Returns the value of the input control used with this instance. <br/>
        /// Return type: <see cref="decimal"/> for <see cref="NumericUpDown"/> | <see cref="string"/>
        /// for <see cref="TextBox"/>.
        /// </summary>
        /// <remarks>May be null if no input control has been specified (yet).</remarks>
        /// <exception cref="NotSupportedException">
        /// Throws if <typeparamref name="T"/> hasn't been implemented for this property.
        /// </exception>
        public U Value => _accessor((T)StylableControls.InputControl);

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
        /// <param name="accessor">accessor to read the current value from the control</param>
        protected StylableInputBox(
            string caption,
            MessageBoxIcon icon,
            string text,
            MessageBoxButtons buttons,
            MessageBoxDefaultButton defaultButton,
            Uri? helpUri,
            TimeSpan? timeout,
            DialogResult timeoutResult,
            T inputControl,
            Func<T, U> accessor
        ) : base(caption, icon, text, buttons, defaultButton, helpUri, timeout, timeoutResult)
        {
            _accessor = accessor;
            StylableControls.InputControl = handleInput(inputControl);
            UpdateSize();
            stretchInputControlWidth(inputControl);
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
        /// Sets width of the input control to full length of the containing window (with margin).
        /// </summary>
        /// <param name="inputControl">Input Control to stretch</param>
        private void stretchInputControlWidth(Control inputControl)
        {
            // Set width to complete width, but leave as much space to the right of the control as to the left.
            inputControl.Width = ClientRectangle.Width - inputControl.Left - inputControl.Margin.Left - inputControl.Margin.Right;
            UpdateSize();
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
    }
}
