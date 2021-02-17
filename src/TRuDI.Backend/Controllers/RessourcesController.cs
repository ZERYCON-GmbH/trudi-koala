namespace TRuDI.Backend.Controllers
{
    using System;

    using Microsoft.AspNetCore.Mvc;

    using Serilog;

    using TRuDI.Backend.Application;

    /// <summary>
    /// This controller is used to get resources like images, css, or javascript files from the adapter. 
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    public class RessourcesController : Controller
    {
        private readonly ApplicationState applicationState;

        /// <summary>
        /// Initializes a new instance of the <see cref="RessourcesController"/> class.
        /// </summary>
        /// <param name="applicationState">State of the application.</param>
        public RessourcesController(ApplicationState applicationState)
        {
            this.applicationState = applicationState;
        }

        /// <summary>
        /// Gets the specified resource file from the active HAN adapter.
        /// </summary>
        /// <param name="path">The path to the resource file.</param>
        /// <returns>The resource file loaded from the assembly or </returns>
        public IActionResult Get(string path)
        {
            try
            {
                var resourceData = this.applicationState.GetResourceFile(path.Substring(10));
                return this.File(resourceData.data, resourceData.contentType);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to loading resource file: {0}", path);
                return this.NotFound();
            }
        }
    }
}
