namespace TRuDI.Backend.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using Serilog;

    using TRuDI.Backend.Application;
    using TRuDI.Backend.Models;
    using TRuDI.Backend.Utils;
    using TRuDI.HanAdapter.Interface;
    using TRuDI.Models;

    public class SupplierFileController : Controller
    {
        private readonly ApplicationState applicationState;

        public SupplierFileController(ApplicationState applicationState)
        {
            this.applicationState = applicationState;
        }

        public IActionResult Index()
        {
            this.applicationState.BreadCrumbTrail.Add("Tarifdaten laden", "/SupplierFile", false);
            this.applicationState.SideBarMenu.Clear();

            return this.View();
        }

        public IActionResult ValidationError()
        {
            return this.View();
        }

        public IActionResult BillingPeriodSelection()
        {
            try
            {
                this.applicationState.LoadSupplierXml();
            }
            catch (Exception)
            {
                return this.RedirectToAction("ValidationError");
            }

            this.applicationState.BreadCrumbTrail.Add("Tarifdaten", "/SupplierFile/BillingPeriodSelection", false);
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadXmlFile(List<IFormFile> files)
        {
            var file = this.Request?.Form?.Files?.FirstOrDefault();
            if (file == null)
            {
                return this.BadRequest("Datei konnte nicht geladen werden.");
            }

            this.applicationState.CurrentSupplierFile = new SupplierFile();
            this.applicationState.CurrentSupplierFile.Data = new MemoryStream();
            this.applicationState.CurrentSupplierFile.Filename = file.FileName;
            await file.CopyToAsync(this.applicationState.CurrentSupplierFile.Data);

            this.applicationState.CurrentSupplierFile.DigestRipemd160 =
                DigestUtils.GetRipemd160(this.applicationState.CurrentSupplierFile.Data);

            this.applicationState.CurrentSupplierFile.DigestSha3 =
                DigestUtils.GetSha3(this.applicationState.CurrentSupplierFile.Data);

            try
            {
                this.applicationState.CurrentSupplierFile.Data.Position = 0;
                this.applicationState.CurrentSupplierFile.Xml = XDocument.Load(this.applicationState.CurrentSupplierFile.Data);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to XML file: {0}", ex.Message);
                this.applicationState.CurrentSupplierFile.Xml = null;
                return this.BadRequest("Datei konnte nicht geladen werden.");
            }

            return this.Ok();
        }


        [HttpPost]
        public async Task<IActionResult> DownloadFile(string url, string username, string password)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrWhiteSpace(username) || !string.IsNullOrWhiteSpace(password))
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                            "Basic",
                            Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{password}")));
                    }

                    var request = new HttpRequestMessage(HttpMethod.Get, url);

                    using (var response = await client.SendAsync(request))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            this.applicationState.CurrentSupplierFile = new SupplierFile();
                            this.applicationState.CurrentSupplierFile.Data =
                                new MemoryStream(await response.Content.ReadAsByteArrayAsync());
                            this.applicationState.CurrentSupplierFile.DownloadUrl = url;
                            this.applicationState.CurrentSupplierFile.Username = username;
                            this.applicationState.CurrentSupplierFile.Password = password;

                            this.applicationState.CurrentSupplierFile.DigestRipemd160 =
                                DigestUtils.GetRipemd160(this.applicationState.CurrentSupplierFile.Data);

                            this.applicationState.CurrentSupplierFile.DigestSha3 =
                                DigestUtils.GetSha3(this.applicationState.CurrentSupplierFile.Data);

                            this.applicationState.CurrentSupplierFile.Data.Position = 0;
                            this.applicationState.CurrentSupplierFile.Xml = XDocument.Load(this.applicationState.CurrentSupplierFile.Data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }

            return this.Ok();
        }

        public IActionResult StartReadout(
            DateTime startTime,
            DateTime endTime)
        {
            var ctx = new AdapterContext
                          {
                              BillingPeriod = null,
                              Start = startTime,
                              End = endTime,
                          };

            if (ctx.Start != this.applicationState.CurrentSupplierFile.Model.AnalysisProfile.BillingPeriod.Start)
            {
                ctx.Start = ctx.Start.DayStart();
            }

            if (ctx.End != this.applicationState.CurrentSupplierFile.Model.AnalysisProfile.BillingPeriod.GetEnd())
            {
                ctx.End = ctx.End.NextDayStart();
            }

            ctx.WithLogdata = true;

            this.applicationState.ConnectData.DeviceId = this.applicationState.CurrentSupplierFile.Model.Smgw.SmgwId;
            this.applicationState.CurrentSupplierFile.Ctx = ctx;

            return this.Ok();
        }
    }
}
