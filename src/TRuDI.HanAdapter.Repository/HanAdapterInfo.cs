namespace TRuDI.HanAdapter.Repository
{
    using System;
    using System.Reflection;

    using TRuDI.HanAdapter.Interface;

    /// <summary>
    /// HAN adapter information. Used by <see cref="HanAdapterRepository"/> to hold the information of an HAN adapter.
    /// </summary>
    public class HanAdapterInfo
    {
        /// <summary>
        /// The HAN adapter type.
        /// </summary>
        private readonly Type adapterType;

        /// <summary>
        /// Initializes a new instance of the <see cref="HanAdapterInfo"/> class.
        /// </summary>
        /// <param name="flagId">The FLAG identifier of the SMGW manufacturer.</param>
        /// <param name="manufacturerName">Name of the SMGW manufacturer.</param>
        /// <param name="adapterType">Type of the adapter.</param>
        public HanAdapterInfo(string flagId, string manufacturerName, Type adapterType)
        {
            this.FlagId = flagId;
            this.ManufacturerName = manufacturerName;

            this.adapterType = adapterType;
        }

        /// <summary>
        /// Gets the FLAG identifier of the SMGW manufacturer.
        /// </summary>
        public string FlagId { get; }

        /// <summary>
        /// Gets the name of the SMGW manufacturer.
        /// </summary>
        public string ManufacturerName { get; }

        /// <summary>
        /// Gets or sets the hash value of the HAN adapter assembly file.
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// Gets the base namespace of the HAN adapter.
        /// </summary>
        public string BaseNamespace => this.adapterType.Namespace;

        /// <summary>
        /// Gets the assembly of the HAN adapter.
        /// </summary>
        public Assembly Assembly => this.adapterType.Assembly;

        /// <summary>
        /// Gets the name of the HAN adapter class.
        /// </summary>
        public string Name => this.adapterType.Name;

        /// <summary>
        /// Creates a instance of the HAN adapter.
        /// </summary>
        /// <returns>Instance of HAN adapter.</returns>
        public IHanAdapter CreateInstance()
        {
            return Activator.CreateInstance(this.adapterType) as IHanAdapter;
        }
    }
}
