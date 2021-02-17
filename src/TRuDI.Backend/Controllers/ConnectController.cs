namespace TRuDI.Backend.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using TRuDI.Backend.Application;
    using TRuDI.Backend.Models;
    using TRuDI.HanAdapter.Repository;

    /// <summary>
    /// The controller of the connect page.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    public class ConnectController : Controller
    {
        private readonly ApplicationState applicationState;

        public ConnectController(ApplicationState applicationState)
        {
            this.applicationState = applicationState;
        }

        public IActionResult Index()
        {
            this.applicationState.BreadCrumbTrail.Add("Verbinden", "/Connect", false);
            this.applicationState.SideBarMenu.Clear();

            this.ViewData["ErrorMessage"] = this.applicationState.LastErrorMessages.FirstOrDefault();
            return this.View(this.applicationState.ConnectData);
        }

        public IActionResult ManufacturerConnectForm(string deviceId)
        {
            try
            {
                var hanAdapter = new HanAdapterContainer(HanAdapterRepository.LoadAdapter(deviceId), deviceId);

                var manufacturerParametersView = hanAdapter.ManufacturerParametersView;
                if (manufacturerParametersView != null)
                {
                    this.ViewData["ManufacturerParametersViewName"] = manufacturerParametersView;
                    return this.PartialView("_ManufacturerParametersFormPartial", deviceId);
                }
            }
            catch (UnknownManufacturerException ex)
            {
                return this.NotFound($"Es wurde kein Smart Meter Gateway-Hersteller mit der ID \"{ex.FlagId}\" gefunden.");
            }
            catch (Exception)
            {
            }

            return this.PartialView("_ManufacturerParametersFormEmptyPartial", deviceId);
        }


        /// <summary>
        /// Gets the additional manufacturer parameters from the request.
        /// </summary>
        /// <returns>A dictionary with the manufacturer specific parameters.</returns>
        private Dictionary<string, string> GetManufacturerParametersFromRequest()
        {
            var manufacturerParameters = new Dictionary<string, string>();
            foreach (var item in this.HttpContext.Request.Form)
            {
                if (item.Key == "__RequestVerificationToken")
                {
                    continue;
                }

                if (typeof(ConnectData).GetProperty(item.Key) == null)
                {
                    manufacturerParameters.Add(item.Key, item.Value);
                }
            }

            return manufacturerParameters;
        }

        [HttpPost]
        public async Task<IActionResult> UploadClientCert(List<IFormFile> files)
        {
            var file = this.Request?.Form?.Files?.FirstOrDefault();
            if (file == null)
            {
                return this.PartialView("_CertWithoutPasswordPartial", null);
            }

            var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            var certData = new CertData(file.FileName, ms.ToArray());
            this.applicationState.ClientCert = certData;

            return this.VerifyCertPassword(this.applicationState.ClientCert.Password);
        }


        [HttpPost]
        public IActionResult UploadXmlFile(List<IFormFile> files)
        {
            this.applicationState.BreadCrumbTrail.Add("Verbinden", "/Connect", true);

            var file = this.Request?.Form?.Files?.FirstOrDefault();
            if (file == null)
            {
                return this.Error();
            }

            XDocument doc;
            using (var stream = file.OpenReadStream())
            {
                try
                {
                    doc = XDocument.Load(stream);
                }
                catch (Exception)
                {
                    this.applicationState.CurrentDataResult.Raw = null;
                    return this.BadRequest();
                }
            }

            try
            {
                this.applicationState.LoadData(doc);
            }
            catch
            {
                return this.BadRequest();
            }

            return this.Ok();
        }


        [HttpPost]
        public IActionResult ValidateClientCertPassword(string password)
        {
            return this.VerifyCertPassword(password);
        }
        
        private PartialViewResult VerifyCertPassword(string password)
        {
            switch (this.applicationState.ClientCert.VerifyPassword(password))
            {
                case CertPasswordState.PasswordValid:
                case CertPasswordState.WithoutPassword:
                    return this.PartialView("_CertWithoutPasswordPartial", this.applicationState.ClientCert);

                case CertPasswordState.InvalidPassword:
                    return this.PartialView("_CertPasswordInputPartial", this.applicationState.ClientCert);

                case CertPasswordState.InvalidCertFile:
                    return this.PartialView("_CertInvalidFilePartial", this.applicationState.ClientCert);
            }

            return this.PartialView("_CertWithoutPasswordPartial", this.applicationState.ClientCert);
        }

        [HttpPost]
        public IActionResult Connect(ConnectData connectData)
        {
            this.applicationState.BreadCrumbTrail.Add("Verbinden", "/Connect", true);

            this.applicationState.ManufacturerParameters = this.GetManufacturerParametersFromRequest();

            connectData.DeviceId = connectData.DeviceId.Trim();
            connectData.Address = connectData.Address.Trim();

            this.applicationState.ConnectData = connectData;

            if (this.applicationState.ConnectData.AuthMode == AuthMode.ClientCertificate)
            {
                if (this.applicationState.ClientCert == null
                    || this.applicationState.ClientCert.PasswordState == CertPasswordState.InvalidCertFile
                    || this.applicationState.ClientCert.PasswordState == CertPasswordState.InvalidPassword
                    || this.applicationState.ClientCert.PasswordState == CertPasswordState.NoCertSelected)
                {
                    this.applicationState.LastErrorMessages.Clear();
                    this.applicationState.LastErrorMessages.Add("Kein gültiges Zertifikat angegeben.");
                    return this.RedirectToAction("Index");
                }
            }

            this.applicationState.ConnectAndLoadContracts();

            return this.RedirectToAction("Index", "Progress");
        }

        public IActionResult Error()
        {
            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}
