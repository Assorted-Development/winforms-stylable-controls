
namespace StylableWinFormsControls.Extensions
{
    /// <summary>
    /// general extension methods
    /// </summary>
    internal static class WinFormExtensions
    {
        /// <summary>
        /// returns all required DialogResults for the given Buttons in a message box
        /// </summary>
        /// <param name="buttons">Buttons that are represented by the resulting <see cref="DialogResult"/>s.</param>
        public static List<DialogResult> ToDialogResult(this MessageBoxButtons buttons)
        {
            List<DialogResult> result = new();
            switch (buttons)
            {
                case MessageBoxButtons.OK:
                    result.Add(DialogResult.OK);
                    break;
                case MessageBoxButtons.OKCancel:
                    result.Add(DialogResult.OK);
                    result.Add(DialogResult.Cancel);
                    break;
                case MessageBoxButtons.AbortRetryIgnore:
                    result.Add(DialogResult.Abort);
                    result.Add(DialogResult.Retry);
                    result.Add(DialogResult.Ignore);
                    break;
                case MessageBoxButtons.YesNoCancel:
                    result.Add(DialogResult.Yes);
                    result.Add(DialogResult.No);
                    result.Add(DialogResult.Cancel);
                    break;
                case MessageBoxButtons.YesNo:
                    result.Add(DialogResult.Yes);
                    result.Add(DialogResult.No);
                    break;
                case MessageBoxButtons.RetryCancel:
                    result.Add(DialogResult.Retry);
                    result.Add(DialogResult.Cancel);
                    break;
                case MessageBoxButtons.CancelTryContinue:
                    result.Add(DialogResult.Cancel);
                    result.Add(DialogResult.Retry);
                    result.Add(DialogResult.Continue);
                    break;
                default:
                    result.Add(DialogResult.OK);
                    break;
            }
            return result;
        }
    }
}
