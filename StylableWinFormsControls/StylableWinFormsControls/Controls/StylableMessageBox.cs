
namespace StylableWinFormsControls
{
    /// <summary>
    /// A stylable version of the <see cref="MessageBox"/>
    /// </summary>
    public sealed partial class StylableMessageBox : Form
    {
        /// <summary>
        /// returns a builder object to configure the <see cref="StylableMessageBox"/>
        /// </summary>
        public static StylableMessageBoxBuilder BUILDER => new();
        /// <summary>
        /// constructor. not available to others as they should use the <see cref="StylableMessageBoxBuilder"/>
        /// </summary>
        private StylableMessageBox()
        {
            InitializeComponent();
        }
        /// <summary>
        /// the checkstate of the checkbox if shown
        /// </summary>
        public CheckState CheckState { get; private set; } = CheckState.Indeterminate;
        /// <summary>
        /// contains the stylable controls for easier access than iterating over Controls
        /// </summary>
        public MessageBoxControls StylableControls { get; private set; }

        /// <summary>
        /// the builder to configure the <see cref="StylableMessageBox"/>
        /// </summary>
#pragma warning disable CA1001 // Types that own disposable fields should be disposable - this is the form in creation which must not be deleted by the builder
        public class StylableMessageBoxBuilder
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
        {
            /// <summary>
            /// the <see cref="StylableMessageBox"/> object currently in creation
            /// </summary>
            private StylableMessageBox _messageBox = new();
            /// <summary>
            /// creates the <see cref="StylableMessageBox"/>
            /// </summary>
            /// <returns>the completely configured but unstyled <see cref="StylableMessageBox"/></returns>
            public StylableMessageBox Build()
            {
                return _messageBox;
            }
            /// <summary>
            /// This adds a <see cref="StylableCheckBox"/> to the form. use <see cref="CheckState"/> to get the check state
            /// </summary>
            /// <param name="checkBoxText">the text to be shown to the user</param>
            public StylableMessageBoxBuilder WithCheckBox(string checkBoxText)
            {
                return this;
            }
            /// <summary>
            /// configures which buttons to show and which button should be the default button
            /// </summary>
            /// <param name="buttons"></param>
            /// <param name="defaultButton"></param>
            public StylableMessageBoxBuilder WithButtons(MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
            {
                return this;
            }
            /// <summary>
            /// Set the messagebox text
            /// </summary>
            /// <param name="text"></param>
            public StylableMessageBoxBuilder WithText(string text)
            {
                return this;
            }
            /// <summary>
            /// Configure the title bar
            /// </summary>
            /// <param name="caption">the caption</param>
            /// <param name="icon">the icon</param>
            public StylableMessageBoxBuilder WithTitle(string caption = "", MessageBoxIcon icon = MessageBoxIcon.None)
            {
                return this;
            }
            /// <summary>
            /// when called, the dialog will close after the given timeout and return the given default result
            /// </summary>
            /// <param name="timeout"></param>
            /// <param name="timeoutDefaultResult"></param>
            public StylableMessageBoxBuilder WithTimeout(TimeSpan timeout, DialogResult timeoutDefaultResult = DialogResult.Cancel)
            {
                return this;
            }
            /// <summary>
            /// Shows the help button in the title bar
            /// </summary>
            /// <param name="url">the url to open when the user clicks on the help button</param>
            public StylableMessageBoxBuilder WithHelpButton(string url)
            {
                _messageBox.HelpButton = true;
                return this;
            }
            /// <summary>
            /// Shows the help button in the title bar
            /// </summary>
            /// <param name="url">the url to open when the user clicks on the help button</param>
            public StylableMessageBoxBuilder WithHelpButton(Uri url)
            {
                _messageBox.HelpButton = true;
                return this;
            }
        }
        /// <summary>
        /// container to provide strongly typed access to all controls of the messageBox for styling
        /// </summary>
        public class MessageBoxControls
        {
            /// <summary>
            /// the label containing the Text
            /// </summary>
            public StylableLabel Text { get; }
            /// <summary>
            /// the optional checkbox
            /// </summary>
            public StylableCheckBox? CheckBox { get; }
            /// <summary>
            /// constructor
            /// </summary>
            /// <param name="text">the label containing the Text</param>
            /// <param name="checkbox">the optional checkbox</param>
            public MessageBoxControls(StylableLabel text, StylableCheckBox? checkbox)
            {
                Text = text;
                CheckBox = checkbox;
            }
        }
    }
}
