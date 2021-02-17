namespace TRuDI.Backend.Components
{
    using Microsoft.AspNetCore.Mvc;

    using TRuDI.Backend.Application;
    using TRuDI.Models;

    public class OriginalValueListErrorsView : ViewComponent
    {
        private readonly ApplicationState applicationState;

        public OriginalValueListErrorsView(ApplicationState applicationState)
        {
            this.applicationState = applicationState;
        }

        public IViewComponentResult Invoke(OriginalValueList ovl)
        {
            return this.View(ovl);
        }
    }
}
