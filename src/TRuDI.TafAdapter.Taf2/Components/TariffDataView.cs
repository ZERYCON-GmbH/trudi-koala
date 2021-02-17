namespace TRuDI.TafAdapter.Taf2.Components
{
    using System;

    using Microsoft.AspNetCore.Mvc;

    using TRuDI.TafAdapter.Interface;
    using TRuDI.TafAdapter.Interface.Taf2;

    public class TariffDataView : ViewComponent
    {
        private readonly ITaf2Data data;

        public TariffDataView(ITafData data)
        {
            this.data = data as ITaf2Data;
        }

        public IViewComponentResult Invoke(DateTime timestamp)
        {
            return this.View(new TariffDataViewModel(timestamp, this.data));
        }
    }
}
