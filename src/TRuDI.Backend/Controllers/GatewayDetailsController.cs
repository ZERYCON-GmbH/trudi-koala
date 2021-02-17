namespace TRuDI.Backend.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using TRuDI.Backend.Application;

    public class GatewayDetailsController : Controller
    {
        private readonly ApplicationState applicationState;

        public GatewayDetailsController(ApplicationState applicationState)
        {
            this.applicationState = applicationState;
        }

        public IActionResult Index()
        {
            this.ViewData["IsGatewayDetails"] = true;
            return this.View();
        }
    }
}
