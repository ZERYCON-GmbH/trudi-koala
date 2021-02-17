namespace TRuDI.TafAdapter.Taf2.Components
{
    using System;

    using TRuDI.TafAdapter.Interface.Taf2;

    public class TariffDataViewModel
    {
        public TariffDataViewModel(DateTime timestamp, ITaf2Data data)
        {
            this.Timestamp = timestamp;
            this.Data = data;
        }

        public DateTime Timestamp { get; }

        public ITaf2Data Data { get; }
    }
}