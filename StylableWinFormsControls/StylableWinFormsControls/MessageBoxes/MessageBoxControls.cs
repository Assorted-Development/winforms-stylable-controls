
using System.Collections.ObjectModel;

namespace StylableWinFormsControls
{
    /// <summary>
    /// container to provide strongly typed access to all controls of the messageBox for styling
    /// </summary>
    public sealed class MessageBoxControls
    {
        /// <summary>
        /// the label containing the Text
        /// </summary>
        public StylableLabel Text { get; init; }
        /// <summary>
        /// the optional checkbox
        /// </summary>
        public StylableCheckBox? CheckBox { get; init; }
        /// <summary>
        /// the optional checkbox
        /// </summary>
        public ReadOnlyCollection<StylableButton> Buttons { get; init; }
        /// <summary>
        /// constructor
        /// </summary>
        public MessageBoxControls() { }
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="text">the label containing the Text</param>
        /// <param name="checkbox">the optional checkbox</param>
        /// <param name="buttons">a list containing all buttons on the messageBox</param>
        public MessageBoxControls(StylableLabel text, StylableCheckBox? checkbox, ReadOnlyCollection<StylableButton> buttons)
        {
            Text = text;
            CheckBox = checkbox;
            Buttons = buttons ?? new ReadOnlyCollection<StylableButton>(new List<StylableButton>());
        }
    }
}
