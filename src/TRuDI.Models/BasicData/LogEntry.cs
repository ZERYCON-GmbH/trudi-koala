namespace TRuDI.Models.BasicData
{
    using System.Diagnostics;

    /// <summary>
    /// Die Klasse LogEntry spezifiziert Logeinträge.
    ///
    /// Eine Instanz der Klasse LogEntry:
    /// 
    /// - kann auf eine Instanz der Klasse LogEvent verweisen
    /// </summary>
    [DebuggerDisplay("{RecordNumber}, {LogEvent.Timestamp}, {LogEvent.Text}, {LogEvent.TLevel}, {LogEvent.TOutcome}")]
    public class LogEntry
    {
        /// <summary>
        /// LogEvent spezifiziert Logereignisse. 
        /// </summary>
        public LogEvent LogEvent 
        {
            get; set;
        }

        /// <summary>
        /// Das Datelement recordNumber ist der eineindeutige Bezeichner des Logeintrags. 
        /// Dieser wird mit Ablegen des Eintrags im Logbuch durch die Geräte-Firmware erzeugt. 
        /// Das Datenelement ist optional anzugeben.
        /// </summary>
        public uint? RecordNumber
        {
            get; set;
        }
    }
}
