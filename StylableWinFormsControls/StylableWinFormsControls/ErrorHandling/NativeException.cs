
namespace StylableWinFormsControls
{
    /// <summary>
    /// Exception when a native call failed
    /// </summary>
    public class NativeException : Exception
    {

        public NativeException(string? message, int hresult, Exception? innerException)
            : base(message, innerException)
        {
            HResult = hresult;
        }

        public NativeException() : this(null, 0, null) { }

        public NativeException(string message) : this(message, 0, null) { }

        public NativeException(string message, Exception innerException) : this(message, 0, innerException) { }

        public NativeException(string message, int hresult) : this(message, hresult, null) { }
    }
}
