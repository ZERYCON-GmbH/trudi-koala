namespace TRuDI.Backend.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using TRuDI.Backend.Application;

    public class ValidationErrorController : Controller
    {
        private readonly ApplicationState applicationState;

        public ValidationErrorController(ApplicationState applicationState)
        {
            this.applicationState = applicationState;
        }

        public IActionResult Index()
        {
#if DEBUG
            this.ViewData["Debug"] = true;
#endif

            this.applicationState.SideBarMenu.Clear();
            return this.View();
        }
    }
}
