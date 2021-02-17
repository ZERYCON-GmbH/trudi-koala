namespace TRuDI.TafAdapter.Taf2.Controllers
{
    using System;

    using Microsoft.AspNetCore.Mvc;

    using TRuDI.TafAdapter.Interface;
    using TRuDI.TafAdapter.Taf2.Components;

    public class Taf2DetailViewController : Controller
    {
        private readonly ITafData data;

        public Taf2DetailViewController(ITafData data)
        {
            this.data = data;
        }

        public ViewComponentResult SelectTariffViewDay(DateTime timestamp)
        {
            return this.ViewComponent(typeof(TariffDataView), new { timestamp = timestamp.Date });
        }
    }
}
