namespace TRuDI.HanAdapter.Repository
{
    using System.Collections.Generic;
    using System.Linq;

    using TRuDI.HanAdapter.Discovergy;
    using TRuDI.HanAdapter.DrNeuhaus;
    using TRuDI.HanAdapter.Efr;
    using TRuDI.HanAdapter.Emh;
    using TRuDI.HanAdapter.Example;
    using TRuDI.HanAdapter.Kiwigrid;
    using TRuDI.HanAdapter.LandisGyr;
    using TRuDI.HanAdapter.Ppc;
    using TRuDI.HanAdapter.ThebenAG;
    using TRuDI.Models;

    /// <summary>
    /// Manages the list of available HAN adapters.
    /// </summary>
    public static class HanAdapterRepository
    {
        /// <summary>
        /// The list of available adapters.
        /// </summary>
        private static readonly List<HanAdapterInfo> availableAdapters = new List<HanAdapterInfo>
            {
                new HanAdapterInfo("DGY", "Discovergy GmbH", typeof(HanAdapterDiscovergy)),
                new HanAdapterInfo("DNT", "Sagemcom Dr. Neuhaus GmbH", typeof(HanAdapterDrNeuhaus)),
                new HanAdapterInfo("DVL", "devolo AG", typeof(HanAdapterKiwigrid)),
                new HanAdapterInfo("EFR", "EFR - Europäische Funk-Rundsteuerung GmbH", typeof(HanAdapterEfr)),
                new HanAdapterInfo("EMH", "EMH metering GmbH & Co. KG", typeof(HanAdapterEmh)),
                new HanAdapterInfo("KIG", "Kiwigrid GmbH", typeof(HanAdapterKiwigrid)),
                new HanAdapterInfo("LGZ", "Landis+Gyr AG", typeof(HanAdapterLandisGyr)),
                new HanAdapterInfo("PPC", "Power Plus Communications AG", typeof(HanAdapterPpc)),
                new HanAdapterInfo("THE", "Theben AG", typeof(HanAdapterThebenAG)),
            };

        /// <summary>
        /// Gets the list available HAN adapters.
        /// </summary>
        public static IReadOnlyList<HanAdapterInfo> AvailableAdapters => availableAdapters;

        /// <summary>
        /// Activates the example/simulation HAN adapter.
        /// </summary>
        public static void ActivateExampleHanAdapter()
        {
            availableAdapters.Add(new HanAdapterInfo("XXX", "Example GmbH", typeof(HanAdapterExample)));
        }

        /// <summary>
        /// Loads the HAN adapter for the specified device identifier.
        /// </summary>
        /// <param name="serverId">The device identifier.</param>
        /// <returns>Instance of <see cref="HanAdapterInfo"/>.</returns>
        /// <exception cref="UnknownManufacturerException">Unknown/unsupported manufacturer FALG id.</exception>
        public static HanAdapterInfo LoadAdapter(string serverId)
        {
            var id = new ServerId(serverId);

            var adapterInfo = AvailableAdapters.FirstOrDefault(a => a.FlagId == id.FlagId);
            if (adapterInfo == null)
            {
                throw new UnknownManufacturerException { FlagId = id.FlagId };
            }

            return adapterInfo;
        }
    }
}
