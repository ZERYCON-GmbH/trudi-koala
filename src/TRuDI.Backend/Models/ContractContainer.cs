namespace TRuDI.Backend.Models
{
    using TRuDI.HanAdapter.Interface;

    /// <summary>
    /// Container class used to group TAF-6 to the corresponding TAF-x contract.
    /// </summary>
    public class ContractContainer
    {
        public ContractInfo Contract { get; set; }
        public ContractInfo Taf6 { get; set; }
    }
}
