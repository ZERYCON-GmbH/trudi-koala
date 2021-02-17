namespace TRuDI.Backend.Utils
{
    using System.Collections.Generic;
    using System.Text;

    using TRuDI.Models;
    using TRuDI.Models.BasicData;

    public static class CsvExport
    {
        public static byte[] ExportLog(IReadOnlyList<LogEntry> logEntries)
        {
            var target = new StringBuilder();

            target.AppendLine("Nr;Zeitstempel;Level;Outcome;Meldungstext");

            if (logEntries != null)
            {
                foreach (var entry in logEntries)
                {
                    target.AppendLine(
                        $"{entry.RecordNumber};{entry.LogEvent.Timestamp};{entry.LogEvent.Level};{entry.LogEvent.Outcome};\"{entry.LogEvent.Text}\"");
                }
            }

            var resultString = target.ToString();
            var resultBytes = new byte[Encoding.UTF8.GetByteCount(resultString) + 3];
            resultBytes[0] = 0xEF;
            resultBytes[1] = 0xBB;
            resultBytes[2] = 0xBF;

            Encoding.UTF8.GetBytes(resultString, 0, resultString.Length, resultBytes, 3);
            return resultBytes;
        }

        public static byte[] ExportOriginalValueList(OriginalValueList ovl)
        {
            var target = new StringBuilder();

            target.AppendLine($"Zeitstempel;Sollzeitstempel;Status PTB;Status FNN;Wert;Einheit;Signatur des SMGWs;Signatur des Sensors;");

            foreach (var block in ovl.MeterReading.IntervalBlocks)
            {
                foreach (var interval in block.IntervalReadings)
                {
                    target.Append($"{interval.CaptureTime};");
                    target.Append($"{interval.TargetTime};");

                    target.Append(interval.StatusPTB.HasValue ? $"{(int)interval.StatusPTB};" : $";");
                    target.Append(interval.StatusFNN != null ? $"{interval.StatusFNN.Status};" : $";");
                    target.Append($"{interval.Value.GetDisplayValue(ovl.MeterReading.ReadingType)};");
                    target.Append($"{ovl.DisplayUnit};");
                    target.Append(!string.IsNullOrWhiteSpace(interval.Signature) ? $"{interval.Signature};" : ";");
                    target.Append(!string.IsNullOrWhiteSpace(interval.MeterSignature) ? $"{interval.MeterSignature};" : ";");
                    target.AppendLine();
                }
            }

            var resultString = target.ToString();
            var resultBytes = new byte[Encoding.UTF8.GetByteCount(resultString) + 3];
            resultBytes[0] = 0xEF;
            resultBytes[1] = 0xBB;
            resultBytes[2] = 0xBF;

            Encoding.UTF8.GetBytes(resultString, 0, resultString.Length, resultBytes, 3);
            return resultBytes;
        }
    }
}
