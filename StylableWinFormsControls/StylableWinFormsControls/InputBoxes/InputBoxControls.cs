
namespace StylableWinFormsControls
{
    /// <summary>
    /// container to provide strongly typed access to all controls of the inputBox for styling
    /// </summary>
    public sealed class InputBoxControls<T> where T : Control
    {
        /// <summary>
        /// the label containing the Text
        /// </summary>
        public StylableLabel Text { get; init; }
        /// <summary>
        /// the 'OK' Button
        /// </summary>
        public StylableButton OkButton { get; init; }
        /// <summary>
        /// the 'Cancel' Button
        /// </summary>
        public StylableButton CancelButton { get; init; }
        /// <summary>
        /// returns all buttons
        /// </summary>
        public StylableButton[] Buttons => new StylableButton[] { OkButton, CancelButton };
        /// <summary>
        /// the control for entering the value
        /// </summary>
        public T InputControl { get; init; }
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="text">the label containing the Text</param>
        /// <param name="okButton"></param>
        /// <param name="cancelButton"></param>
        public InputBoxControls(StylableLabel text, StylableButton okButton, StylableButton cancelButton, T inputControl)
        {
            Text = text;
            OkButton = okButton;
            CancelButton = cancelButton;
            InputControl = inputControl;
        }
    }
}
