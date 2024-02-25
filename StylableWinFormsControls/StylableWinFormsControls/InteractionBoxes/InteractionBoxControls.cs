using System.Collections.ObjectModel;

namespace StylableWinFormsControls
{
    public class InteractionBoxControls<T> where T : Control
    {
        /// <summary>
        /// the label containing the Text
        /// </summary>
        public StylableLabel? Text { get; internal set; }
        /// <summary>
        /// the optional checkbox
        /// </summary>
        public ReadOnlyCollection<StylableButton> Buttons { get; internal set; } = new(Array.Empty<StylableButton>());

        /// <summary>
        /// the control for entering a value
        /// </summary>
        public T? InputControl { get; internal set; }

        /// <summary>
        /// constructor
        /// </summary>
        internal InteractionBoxControls() { }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="text">the label containing the Text</param>
        /// <param name="inputControl">the optional input control</param>
        /// <param name="buttons">a list containing all buttons on the messageBox</param>
        internal InteractionBoxControls(
            StylableLabel text,
            T? inputControl,
            ReadOnlyCollection<StylableButton> buttons)
        {
            Text = text;
            InputControl = inputControl;
            Buttons = buttons ?? new ReadOnlyCollection<StylableButton>(new List<StylableButton>());
        }
    }
}
