namespace TRuDI.Models
{
    using System;

    public class DailyOvlErrorStatus
    {
        public DateTime Timestamp { get; set; }

        public int ValueCount { get; set; }
        public int GapCount { get; set; }
        public int FatalErrorCount { get; set; }
        public int CriticalTempErrorCount { get; set; }
        public int TempErrorCount { get; set; }
        public int WarningCount { get; set; }
        public int OkCount { get; set; }

        public bool HasErrors => this.GapCount > 0 || this.OkCount != this.ValueCount;
    }
}