namespace TRuDI.Backend.Components
{
    using System;

    using Microsoft.AspNetCore.Mvc;

    using TRuDI.Backend.Application;
    using TRuDI.Models;
    using TRuDI.Models.BasicData;

    public class OriginalValueListView : ViewComponent
    {
        private readonly ApplicationState applicationState;

        public OriginalValueListView(ApplicationState applicationState)
        {
            this.applicationState = applicationState;
        }

        public IViewComponentResult Invoke(OriginalValueList ovl, DateTime startTime)
        {
            startTime = startTime.Date;
            if (ovl.ServiceCategory == Kind.Gas)
            {
                startTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, 6, 0, 0, startTime.Kind);
            }

            var endTime = startTime + TimeSpan.FromDays(1);

            var items = ovl.GetReadings(startTime, endTime);

            return this.View(new OriginalValueListRange(startTime, endTime, ovl, items));
        }
    }
}
