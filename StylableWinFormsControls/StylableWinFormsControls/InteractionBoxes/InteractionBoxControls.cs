using System.Collections.ObjectModel;

namespace StylableWinFormsControls
{
    public class InteractionBoxControls
    {
        /// <summary>
        /// the label containing the Text
        /// </summary>
        public StylableLabel? Text { get; internal set; }
        /// <summary>
        /// the optional checkbox
        /// </summary>
        public StylableCheckBox? CheckBox { get; internal set; }
        /// <summary>
        /// the optional checkbox
        /// </summary>
        public ReadOnlyCollection<StylableButton> Buttons { get; internal set; } = new(Array.Empty<StylableButton>());

        /// <summary>
        /// the control for entering a value
        /// </summary>
        public Control InputControl { get; internal set; }

        /// <summary>
        /// constructor
        /// </summary>
        internal InteractionBoxControls() { }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="text">the label containing the Text</param>
        /// <param name="checkbox">the optional checkbox</param>
        /// <param name="buttons">a list containing all buttons on the messageBox</param>
        internal InteractionBoxControls(
            StylableLabel text,
            StylableCheckBox? checkbox,
            ReadOnlyCollection<StylableButton> buttons)
        {
            Text = text;
            CheckBox = checkbox;
            Buttons = buttons ?? new ReadOnlyCollection<StylableButton>(new List<StylableButton>());
        }
    }
}
