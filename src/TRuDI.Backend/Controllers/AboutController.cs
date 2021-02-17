namespace TRuDI.Backend.Controllers
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.AspNetCore.Mvc;

    using TRuDI.Backend.Application;
    using TRuDI.HanAdapter.Repository;
    using TRuDI.TafAdapter.Repository;

    public class AboutController : Controller
    {
        private readonly ApplicationState applicationState;

        public AboutController(ApplicationState applicationState)
        {
            this.applicationState = applicationState;
        }

        public IActionResult Index()
        {
            var res = this.View();
            return res;
        }

        public List<ApplicationChecksums.DigestItem> GetDigestItems()
        {
            var items = new List<ApplicationChecksums.DigestItem>();
            items.AddRange(this.applicationState.BackendChecksums.Items);
            items.AddRange(HanAdapterRepository.AvailableAdapters.Select(a => new ApplicationChecksums.DigestItem(a)));
            items.AddRange(TafAdapterRepository.AvailableAdapters.Select(a => new ApplicationChecksums.DigestItem(a)));

            return items;
        }
    }
}
