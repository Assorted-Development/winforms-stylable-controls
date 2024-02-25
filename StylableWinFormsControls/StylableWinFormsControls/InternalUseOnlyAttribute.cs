
namespace StylableWinFormsControls
{
    /// <summary>
    /// marks a method or property as internal only. this can be used when inherited properties should not be used from outside callers
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class InternalUseOnlyAttribute : Attribute
    {
        /// <summary>
        /// the description why this should not be used
        /// </summary>
        public string Description { get; }
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="description">the description why this should not be used</param>
        public InternalUseOnlyAttribute(string description)
        {
            Description = description;
        }
    }
}
