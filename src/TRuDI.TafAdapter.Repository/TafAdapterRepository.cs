namespace TRuDI.TafAdapter.Repository
{
    using System.Collections.Generic;
    using System.Linq;

    using TRuDI.HanAdapter.Interface;
    using TRuDI.TafAdapter.Interface;
    using TRuDI.TafAdapter.Taf1;
    using TRuDI.TafAdapter.Taf2;

    /// <summary>
    /// Manages the list of available TAF adapters.
    /// </summary>
    public static class TafAdapterRepository
    {
        /// <summary>
        /// The list of available adapters.
        /// </summary>
        private static readonly List<TafAdapterInfo> availableAdapters = new List<TafAdapterInfo>
            {
                new TafAdapterInfo(TafId.Taf1, "Standard Adapter für TAF-1", typeof(TafAdapterTaf1)),
                new TafAdapterInfo(TafId.Taf2, "Standard Adapter für TAF-2", typeof(TafAdapterTaf2)),
            };

        /// <summary>
        /// Gets the list available TAF adapters.
        /// </summary>
        public static IReadOnlyList<TafAdapterInfo> AvailableAdapters => availableAdapters;

        /// <summary>
        /// Loads the TAF adapter for the specified TAF id.
        /// </summary>
        /// <param name="tafId">The TAF identifier.</param>
        /// <returns>Instance of the TAF adapter.</returns>
        /// <exception cref="UnknownTafAdapterException">No TAF adapter for the specified TAF id.</exception>
        public static ITafAdapter LoadAdapter(TafId tafId)
        {
            var adapterInfo = AvailableAdapters.FirstOrDefault(a => a.TafId == tafId);
            if (adapterInfo == null)
            {
                throw new UnknownTafAdapterException { TafId = tafId };
            }

            return adapterInfo.CreateInstance();
        }
    }
}
