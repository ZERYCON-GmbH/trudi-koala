namespace TRuDI.Backend.Application
{
    /// <summary>
    /// The operation mode of the application.
    /// </summary>
    public enum OperationMode
    {
        /// <summary>
        /// The operation mode is not selected by the user.
        /// </summary>
        NotSelected,

        /// <summary>
        /// The display function was selected.
        /// </summary>
        DisplayFunction,

        /// <summary>
        /// The transparency function was selected.
        /// </summary>
        TransparencyFunction,
    }
}