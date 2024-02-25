
namespace StylableWinFormsControls
{
    /// <summary>
    /// the builder to configure the <see cref="StylableInputBox"/>
    /// </summary>
    public sealed class StylableInputBoxBuilder : StylableInteractionBoxBuilder<StylableInputBoxBuilder>
    {
        /// <summary>
        /// creates the <see cref="StylableInputBox"/> for text input
        /// </summary>
        /// <param name="startValue">the value to show at the start</param>
        /// <returns>the completely configured but unstyled <see cref="StylableInputBox{TextBox}"/></returns>
        public StylableTextInputBox ForText(string startValue = "")
        {
            return new StylableTextInputBox(Caption, Icon, Text, Buttons, DefaultButton, HelpUri, Timeout, TimeoutResult, startValue);
        }

        /// <summary>
        /// creates the <see cref="StylableInputBox"/> for numeric input
        /// </summary>
        /// <returns>the completely configured but unstyled <see cref="StylableNumericInputBox"/></returns>
        public StylableNumericInputBox ForNumericValue(decimal startValue = 0, decimal minValue = decimal.MinValue, decimal maxValue = decimal.MaxValue)
        {
            return new StylableNumericInputBox(Caption, Icon, Text, Buttons, DefaultButton, HelpUri, Timeout, TimeoutResult, startValue, minValue, maxValue);
        }

    }
}
