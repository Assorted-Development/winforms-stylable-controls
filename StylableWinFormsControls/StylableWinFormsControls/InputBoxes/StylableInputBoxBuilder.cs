using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StylableWinFormsControls.InputBoxes
{
    /// <summary>
    /// the builder to configure the <see cref="StylableInputBox"/>
    /// </summary>
    public sealed class StylableInputBoxBuilder
    {
        /// <summary>
        /// the caption
        /// </summary>
        private string _caption = string.Empty;
        /// <summary>
        /// the icon in the title bar
        /// </summary>
        private MessageBoxIcon _icon = MessageBoxIcon.None;
        /// <summary>
        /// the prompt text
        /// </summary>
        private string _text = string.Empty;
        /// <summary>
        /// defines which button should be selected by default
        /// </summary>
        private MessageBoxDefaultButton _defaultButton = MessageBoxDefaultButton.Button1;
        /// <summary>
        /// the url to open when the user clicks on the help button
        /// </summary>
        private Uri? _helpUri;
        /// <summary>
        /// defines the intervall after which the inputBox is closed automatically
        /// </summary>
        private TimeSpan? _timeout;
        /// <summary>
        /// defines the <see cref="DialogResult"/> to return when the timeout hits
        /// </summary>
        private DialogResult _timeoutResult = DialogResult.None;
        /// <summary>
        /// creates the <see cref="StylableInputBox"/> for text input
        /// </summary>
        /// <param name="startValue">the value to show at the start</param>
        /// <returns>the completely configured but unstyled <see cref="StylableInputBox"/></returns>
        public StylableInputBox<TextBox> ForText(string startValue = "")
        {
            TextBox textBox = new()
            {
                Text = startValue
            };
            return new StylableInputBox<TextBox>(_caption, _icon, _text, _defaultButton, _helpUri, _timeout, _timeoutResult, textBox);
        }
        /// <summary>
        /// creates the <see cref="StylableInputBox"/> for text input
        /// </summary>
        /// <returns>the completely configured but unstyled <see cref="StylableInputBox"/></returns>
        public StylableInputBox<NumericUpDown> ForNumericValue(decimal startValue = 0, decimal minValue = decimal.MinValue, decimal maxValue = decimal.MaxValue)
        {
            NumericUpDown nup = new()
            {
                Minimum = minValue,
                Maximum = maxValue,
                Value = startValue
            };
            return new StylableInputBox<NumericUpDown>(_caption, _icon, _text, _defaultButton, _helpUri, _timeout, _timeoutResult, nup);
        }
        /// <summary>
        /// Set the prompt text of the InputBox
        /// </summary>
        /// <param name="text"></param>
        public StylableInputBoxBuilder WithText(string text)
        {
            _text = text ?? string.Empty;
            return this;
        }
        /// <summary>
        /// Configure the title bar
        /// </summary>
        /// <param name="caption">the caption</param>
        /// <param name="icon">the icon</param>
        public StylableInputBoxBuilder WithTitle(string caption = "", MessageBoxIcon icon = MessageBoxIcon.None)
        {
            _caption = caption ?? string.Empty;
            _icon = icon;
            return this;
        }
        /// <summary>
        /// when called, the dialog will close after the given timeout and return the given default result
        /// </summary>
        /// <param name="timeout">defines the intervall after which the inputbox is closed automatically</param>
        /// <param name="timeoutResult">defines the <see cref="DialogResult"/> to return when the timeout hits</param>
        public StylableInputBoxBuilder WithTimeout(TimeSpan timeout, DialogResult timeoutResult = DialogResult.Cancel)
        {
            _timeout = timeout;
            _timeoutResult = timeoutResult;
            return this;
        }
        /// <summary>
        /// Shows the help button in the title bar
        /// </summary>
        /// <param name="url">the url to open when the user clicks on the help button</param>
        /// <exception cref="ArgumentNullException">url must be non-null</exception>
        public StylableInputBoxBuilder WithHelpButton(string url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }
            return WithHelpButton(new Uri(url));
        }
        /// <summary>
        /// Shows the help button in the title bar
        /// </summary>
        /// <param name="url">the url to open when the user clicks on the help button</param>
        /// <exception cref="ArgumentNullException">url must be non-null</exception>
        public StylableInputBoxBuilder WithHelpButton(Uri url)
        {
            if (url is null)
            {
                throw new ArgumentNullException(nameof(url));
            }
            _helpUri = url;
            return this;
        }
    }
}
