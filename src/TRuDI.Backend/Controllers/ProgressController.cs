namespace TRuDI.Backend.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using TRuDI.Backend.Application;

    public class ProgressController : Controller
    {
        private readonly ApplicationState applicationState;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressController"/> class.
        /// </summary>
        /// <param name="applicationState">State of the application.</param>
        public ProgressController(ApplicationState applicationState)
        {
            this.applicationState = applicationState;
        }

        public IActionResult Index()
        {
            this.ViewData["IsProgressPage"] = true;
#if DEBUG
            this.ViewData["WebSocketProtocol"] = "ws:";
#else
            this.ViewData["WebSocketProtocol"] = "wss:";
#endif
            return this.View(this.applicationState.CurrentProgressState);
        }

        [HttpPost]
        public IActionResult CancelOperation()
        {
            this.applicationState.CancelOperation();
            return this.Ok();
        }

        [HttpGet]
        public IActionResult GetNextPageToLoad()
        {
            if (this.applicationState.CurrentProgressState.NextPageAfterProgress != null)
            {
                return this.Ok(this.applicationState.CurrentProgressState.NextPageAfterProgress);
            }

            return this.NotFound();
        }
    }
}
