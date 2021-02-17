namespace TRuDI.Backend.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using TRuDI.Backend.Application;

    public class ErrorController : Controller
    {
        private readonly ApplicationState applicationState;

        public ErrorController(ApplicationState applicationState)
        {
            this.applicationState = applicationState;
        }

        public IActionResult Index()
        {
            this.applicationState.SideBarMenu.Clear();
            return this.View();
        }

        public IActionResult InternalError()
        {
            this.ViewData["InternalError"] = true;
            this.applicationState.Reset();
            return this.View("~/Views/OperatingModeSelection/Index.cshtml");
        }
    }
}
