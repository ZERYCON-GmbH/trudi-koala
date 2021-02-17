namespace TRuDI.Backend.Exceptions
{
    using System;

    using TRuDI.HanAdapter.Interface;

    /// <summary>
    /// Exception based on an <see cref="AdapterError"/> returned from a HAN adapter.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class HanAdapterException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HanAdapterException"/> class.
        /// </summary>
        /// <param name="error">The adapter error.</param>
        public HanAdapterException(AdapterError error)
        {
            this.AdapterError = error;
        }

        /// <summary>
        /// Gets the adapter error.
        /// </summary>
        public AdapterError AdapterError { get; }

        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        public override string Message => $"{this.AdapterError.Type} - {this.AdapterError.Message}";
    }
}
