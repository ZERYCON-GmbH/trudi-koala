namespace TRuDI.TafAdapter.Repository
{
    using System;
    using System.Reflection;

    using TRuDI.HanAdapter.Interface;
    using TRuDI.TafAdapter.Interface;

    /// <summary>
    /// TAF adapter information. Used by <see cref="TafAdapterRepository"/> to hold the information of an TAF adapter.
    /// </summary>
    public class TafAdapterInfo
    {
        /// <summary>
        /// The TAF adapter type.
        /// </summary>
        private readonly Type adapterType;

        /// <summary>
        /// Initializes a new instance of the <see cref="TafAdapterInfo"/> class.
        /// </summary>
        /// <param name="tafId">The TAF identifier.</param>
        /// <param name="description">The adapter description.</param>
        /// <param name="adapterType">Type of the adapter.</param>
        public TafAdapterInfo(TafId tafId, string description, Type adapterType)
        {
            this.TafId = tafId;
            this.Description = description;

            this.adapterType = adapterType;
        }

        /// <summary>
        /// Gets the TAF identifier.
        /// </summary>
        public TafId TafId { get; }

        /// <summary>
        /// Gets the TAF adapter description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets or sets the hash value of the TAF adapter assembly file.
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// Gets the base namespace of the TAF adapter.
        /// </summary>
        public string BaseNamespace => this.adapterType.Namespace;

        /// <summary>
        /// Gets the assembly of the TAF adapter.
        /// </summary>
        public Assembly Assembly => this.adapterType.Assembly;

        /// <summary>
        /// Creates a instance of the TAF adapter.
        /// </summary>
        /// <returns>Instance of TAF adapter.</returns>
        public ITafAdapter CreateInstance()
        {
            return Activator.CreateInstance(this.adapterType) as ITafAdapter;
        }
    }
}
