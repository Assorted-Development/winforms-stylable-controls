using System.Diagnostics;
using System.Reflection;

namespace StylableWinFormsControls
{
    /// <summary>
    /// Settings that can be set from the caller
    /// </summary>
    public class StylableWinFormsControlsSettings
    {
        /// <summary>
        /// the default settings object. will be used if no settings object was provided to the controls
        /// </summary>
        public static readonly StylableWinFormsControlsSettings DEFAULT = new();
        /// <summary>
        /// defines how the library should handle errors
        /// </summary>
        public ErrorHandling ErrorHandling { get; set; } = ErrorHandling.Default;
        /// <summary>
        /// returns true if a failed call should raise an exception
        /// </summary>
        public bool IsErrorHandlingFail()
        {
            return ErrorHandling == ErrorHandling.Fail ||
                (ErrorHandling == ErrorHandling.Default && isAssemblyDebugBuild(Assembly.GetEntryAssembly()));
        }
        /// <summary>
        /// returns true if the Assembly is a debug build
        /// </summary>
        /// <param name="assembly"></param>
        private static bool isAssemblyDebugBuild(Assembly? assembly)
        {
            if (assembly is null)
            {
                //GetEntryAssembly is true for usecases like unittests so we handle this as Debug builds
                return true;
            }
            foreach (object attribute in assembly.GetCustomAttributes(false))
            {
                if (attribute is DebuggableAttribute debuggableAttribute)
                {
                    return debuggableAttribute.IsJITTrackingEnabled;
                }
            }
            return false;
        }
    }
}
