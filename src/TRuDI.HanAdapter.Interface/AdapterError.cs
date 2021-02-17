namespace TRuDI.HanAdapter.Interface
{
    /// <summary>
    /// Container for an error type and error message.
    /// </summary>
    public class AdapterError
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdapterError"/> class.
        /// </summary>
        /// <param name="type">The error type.</param>
        public AdapterError(ErrorType type)
        {
            this.Type = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdapterError"/> class.
        /// </summary>
        /// <param name="type">The error type.</param>
        /// <param name="message">The error message.</param>
        public AdapterError(ErrorType type, string message)
        {
            this.Type = type;
            this.Message = message;
        }

        /// <summary>
        /// Gets the error type.
        /// </summary>
        public ErrorType Type { get; }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(this.Message))
            {
                return $"{this.Type}";
            }

            return $"{this.Type} - {this.Message}";
        }
    }
}