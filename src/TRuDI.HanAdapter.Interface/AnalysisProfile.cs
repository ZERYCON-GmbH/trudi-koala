namespace TRuDI.HanAdapter.Interface
{
    using System.Xml.Linq;

    /// <summary>
    /// Analysis profile XML data.
    /// </summary>
    public class AnalysisProfile
    {
        /// <summary>
        /// The Xml file which contains the relevant supplier data for Taf-7
        /// </summary>
        public XDocument SupplierData
        {
            get; set;
        }
    }
}