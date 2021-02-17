namespace TRuDI.Backend.Components
{
    using System;
    using System.Linq;

    using Microsoft.AspNetCore.Mvc;

    using TRuDI.Backend.Application;
    using TRuDI.Backend.Utils;
    using TRuDI.Models;

    public class LogItemsView : ViewComponent
    {
        private readonly ApplicationState applicationState;

        public LogItemsView(ApplicationState applicationState)
        {
            this.applicationState = applicationState;
        }

        public IViewComponentResult Invoke(DateTime startTime, DateTime endTime, string filterText)
        {
            startTime = startTime.DayStart();

            if (endTime == DateTime.MinValue)
            {
                endTime = DateTime.MaxValue;
            }
            else
            {
                endTime = endTime.DayEnd();
            }

            if (!string.IsNullOrWhiteSpace(filterText))
            {
                filterText = filterText.ToLowerInvariant();
            }

            return this.View(this.applicationState.CurrentDataResult.Model.LogEntries.Where(
                e => e.LogEvent != null && e.LogEvent.Timestamp >= startTime && e.LogEvent.Timestamp <= endTime
                     && (string.IsNullOrWhiteSpace(filterText)
                         || (e.LogEvent.Text.ToLowerInvariant().Contains(filterText) || e.LogEvent.Level
                                 .GetLogLevelString().ToLowerInvariant().Contains(filterText)))));
        }
    }
}
