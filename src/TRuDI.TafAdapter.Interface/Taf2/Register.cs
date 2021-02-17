namespace TRuDI.TafAdapter.Interface.Taf2
{
    using System.Diagnostics;

    using TRuDI.Models;
    using TRuDI.Models.BasicData;

    [DebuggerDisplay("{ObisCode}, TariffId={TariffId}, Amount={Amount}")]
    public class Register
    {
        public Register()
        {
        }

        /// <summary>
        /// The reading type from the original value list used as source of this register.
        /// </summary>
        public ReadingType SourceType { get; set; }

        public ObisId ObisCode { get; set; }
        public ushort TariffId { get; set; }
    
        public long? Amount
        {
            get; set;
        }
    }
}
