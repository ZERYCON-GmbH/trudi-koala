namespace TRuDI.Backend.Components
{
    using System.Security.Cryptography.X509Certificates;

    using Microsoft.AspNetCore.Mvc;

    using TRuDI.Backend.Application;

    public class CertDetailsView : ViewComponent
    {
        private readonly ApplicationState applicationState;

        public CertDetailsView(ApplicationState applicationState)
        {
            this.applicationState = applicationState;
        }

        public IViewComponentResult Invoke(X509Certificate2 cert)
        {
            return this.View(cert);
        }
    }
}
