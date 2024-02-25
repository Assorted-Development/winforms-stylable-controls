
namespace StylableWinFormsControls
{
    /// <summary>
    /// A stylable version of  VB.NETs Interaction.InputBox for numbers
    /// </summary>
    public class StylableNumericInputBox : StylableInputBox<NumericUpDown, decimal>
    {
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
        /// <param name="startValue"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        internal StylableNumericInputBox(
            string caption,
            MessageBoxIcon icon,
            string text,
            MessageBoxButtons buttons,
            MessageBoxDefaultButton defaultButton,
            Uri? helpUri,
            TimeSpan? timeout,
            DialogResult timeoutResult,
            decimal startValue,
            decimal minValue,
            decimal maxValue
        ) : base(caption, icon, text, buttons, defaultButton, helpUri, timeout, timeoutResult, new NumericUpDown(), nup => nup.Value)
        {

            StylableControls.InputControl!.Minimum = minValue;
            StylableControls.InputControl!.Maximum = maxValue;
            StylableControls.InputControl!.Value = startValue;
        }
    }
}
