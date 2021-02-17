namespace TRuDI.Backend.Controllers
{
    using System;
    using System.IO;
    using System.Linq;

    using Microsoft.AspNetCore.Mvc;

    using TRuDI.Backend.Application;
    using TRuDI.Backend.Components;
    using TRuDI.Backend.Utils;

    public class DataViewController : Controller
    {
        private readonly ApplicationState applicationState;

        public DataViewController(ApplicationState applicationState)
        {
            this.applicationState = applicationState;
        }

        public IActionResult Index()
        {
            this.applicationState.BreadCrumbTrail.Add("Abrechnungsdaten", "/DataView", false);
            this.applicationState.SideBarMenu.Clear();
            this.applicationState.SideBarMenu.Add(null, null);
            this.applicationState.SideBarMenu.Add("Zertifikate", "/CertificateDetails");
            this.applicationState.SideBarMenu.Add("Daten exportieren", "$('#exportSelectionDialog').modal('show');", useOnClick: true);

            return this.View();
        }

#if DEBUG
        /// <summary>
        /// Returns the export data to the browser.
        /// </summary>
        /// <param name="exportType">Specifies the export type.</param>
        /// <returns>A FileResult with the export data.</returns>
        [HttpGet("/DataView/Export/{exportType}")]
        public IActionResult Export(string exportType)
        {
            switch (exportType)
            {
                case "XML":
                    var ms = new MemoryStream();
                    this.applicationState.CurrentDataResult.VersionedExportXml.Save(ms);
                    ms.Position = 0;

                    this.Response.Headers.Add("Content-Disposition", "attachment; filename=result.xml");
                    return new FileStreamResult(ms, "text/xml");

                case "CSV_LOG_ITEMS":
                    this.Response.Headers.Add("Content-Disposition", "attachment; filename=export.csv");
                    return new FileContentResult(CsvExport.ExportLog(this.applicationState.CurrentDataResult?.Model?.LogEntries), "text/csv");

                default:
                    var selectedOvl = this.applicationState.CurrentDataResult.OriginalValueLists.FirstOrDefault(
                        ovl => ovl.GetOriginalValueListIdent() == exportType);

                    if (selectedOvl != null)
                    {
                        this.Response.Headers.Add("Content-Disposition", "attachment; filename=export.csv");
                        return new FileContentResult(CsvExport.ExportOriginalValueList(selectedOvl), "text/csv");
                    }

                    break;
            }

            return this.NotFound();
        }
#endif

        /// <summary>
        /// Writes the export data directly to the specified file.
        /// </summary>
        /// <param name="exportType">Type of the export.</param>
        /// <param name="filename">The filename.</param>
        /// <returns>OK on success.</returns>
        [HttpPost("/DataView/ExportToFile/{exportType}")]
        public IActionResult Export(string exportType, string filename)
        {
            try
            {
                byte[] exportData = null;

                switch (exportType)
                {
                    case "XML":
                        var ms = new MemoryStream();
                        this.applicationState.CurrentDataResult.VersionedExportXml.Save(ms);
                        exportData = ms.ToArray();
                        break;

                    case "CSV_LOG_ITEMS":
                        exportData = CsvExport.ExportLog(this.applicationState.CurrentDataResult?.Model?.LogEntries);
                        break;

                    default:
                        var selectedOvl = this.applicationState.CurrentDataResult.OriginalValueLists.FirstOrDefault(
                            ovl => ovl.GetOriginalValueListIdent() == exportType);

                        if (selectedOvl != null)
                        {
                            exportData = CsvExport.ExportOriginalValueList(selectedOvl);
                        }

                        break;
                }

                if (exportData != null)
                {
                    System.IO.File.WriteAllBytes(filename, exportData);
                }
            }
            catch (Exception)
            {
            }

            return this.Ok();
        }

        public ViewComponentResult FilterLog(DateTime startTime, DateTime endTime, string filterText, string filterLevel)
        {
            return this.ViewComponent(typeof(LogItemsView), new { startTime = startTime, endTime = endTime, filterText, filterLevel });
        }

        public ViewComponentResult FilterOvl(string ovlId, DateTime startTime)
        {
            var ovl = this.applicationState.CurrentDataResult.OriginalValueLists.FirstOrDefault(
                l => l.GetOriginalValueListIdent() == ovlId);

            return this.ViewComponent(typeof(OriginalValueListView), new { ovl, startTime });
        }

        public ViewComponentResult ShowErrorsList(string ovlId)
        {
            var ovl = this.applicationState.CurrentDataResult.OriginalValueLists.FirstOrDefault(
                l => l.GetOriginalValueListIdent() == ovlId);

            return this.ViewComponent(typeof(OriginalValueListErrorsView), new { ovl });
        }
    }
}
