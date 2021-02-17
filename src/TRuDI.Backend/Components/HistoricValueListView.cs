namespace TRuDI.Backend.Components
{
    using Microsoft.AspNetCore.Mvc;

    using TRuDI.Backend.Application;
    using TRuDI.Models;

    public class HistoricValueListView : ViewComponent
    {
        private readonly ApplicationState applicationState;

        public HistoricValueListView(ApplicationState applicationState)
        {
            this.applicationState = applicationState;
        }

        public IViewComponentResult Invoke(OriginalValueList ovl)
        {
            return this.View(ovl);
        }
    }
}
