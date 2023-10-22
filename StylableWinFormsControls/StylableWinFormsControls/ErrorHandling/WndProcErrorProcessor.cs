namespace StylableWinFormsControls
{
    /// <summary>
    /// Utility class for error handling
    /// </summary>
    internal class WndProcErrorProcessor
    {
        /// <summary>
        /// a delegate matching the WndProc interface
        /// </summary>
        /// <param name="m"></param>
        public delegate void WndProcAction(ref Message m);
        /// <summary>
        /// the settings object to get the error handling info
        /// </summary>
        private readonly StylableWinFormsControlsSettings _settings;
        /// <summary>
        /// the WndProc to call
        /// </summary>
        private readonly WndProcAction _main;
        /// <summary>
        /// the WndProc fallback to call when main throws an error
        /// </summary>
        private readonly WndProcAction _fallback;
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="settings">the settings object to get the error handling info</param>
        /// <param name="main">the WndProc to call</param>
        /// <param name="fallback">the WndProc fallback to call when main throws an error</param>
        public WndProcErrorProcessor(StylableWinFormsControlsSettings settings, WndProcAction main, WndProcAction fallback)
        {
            _settings = settings;
            _main = main;
            _fallback = fallback;
        }
        /// <summary>
        /// run the WndProc method and do error handling
        /// </summary>
        /// <param name="m"></param>
        /// <exception cref="NativeException">when main raised an error and the errorhandling is set to fail</exception>
        public void WndProc(ref Message m)
        {
            try
            {
                _main(ref m);
            }
            catch (NativeException ex)
            {
                if (_settings.IsErrorHandlingFail())
                {
                    throw ex;
                }
                _fallback(ref m);
            }
        }
    }
}
