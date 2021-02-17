namespace TRuDI.Backend.Models
{
    using System.IO;
    using System.Xml.Linq;

    using TRuDI.HanAdapter.Interface;
    using TRuDI.Models.BasicData;
    using TRuDI.TafAdapter.Interface;

    /// <summary>
    /// A instance of this class holds the supplier file data used by the transparency function.
    /// </summary>
    public class SupplierFile
    {
        public string Filename { get; set; }
        public string DownloadUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public MemoryStream Data { get; set; }
        public XDocument Xml { get; set; }
        public UsagePointLieferant Model { get; set; }

        public string DigestRipemd160 { get; set; }
        public string DigestSha3 { get; set; }

        public AdapterContext Ctx { get; set; }

        /// <summary>
        /// Contains the summary, the detail view and the calculate data that is displayed by the views.
        /// </summary>
        public TafAdapterData TafData { get; set; }

        /// <summary>
        /// Gets the TAF identifier from the analysis profile if it exists within the data.
        /// </summary>
        public TafId? TafId => this.Model?.AnalysisProfile?.TariffUseCase;
    }
}