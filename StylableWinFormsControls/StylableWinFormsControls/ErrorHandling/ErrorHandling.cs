namespace StylableWinFormsControls
{
    /// <summary>
    /// defines how the library should handle errors
    /// </summary>
    public enum ErrorHandling
    {
        /// <summary>
        /// uses the value depending on the build 
        /// </summary>
        Default = 0,
        /// <summary>
        /// throws an exception. recommended for debug builds to find errors early
        /// </summary>
        Fail = 1,
        /// <summary>
        /// continue without exception and fallback to .NET rendering. Recommended for release builds
        /// to not interrupt the user
        /// </summary>
        Continue = 2
    }
}
