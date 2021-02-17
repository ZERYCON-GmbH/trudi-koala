namespace TRuDI.Models.BasicData
{
    using System.Collections.Generic;

    /// <summary>
    /// Die Klasse UsagePoint repräsentiert den Zählpunkt und stellt das zentrale Datenelement 
    /// einer Nachricht dar.Jede Nachricht muss mindestens einen Zählpunkt beinhalten.
    /// 
    /// Eine Instanz der Klasse UsagePoint:
    ///	    muss auf eine Instanz der Klasse InvoicingParty verweisen
    ///	    
    ///	    muss auf eine Instanz der Klasse Customer verweisen
    ///	    
    ///	    muss auf eine Instanz der Klasse SMGW verweisen
    ///	    
    ///	    muss auf eine Instanz der Klasse ServiceCategory verweisen
    ///	    
    ///	    muss auf mindestens eine Instanz der Klasse MeterReading verweisen
    ///	    
    ///	    kann auf Instanzen der Klasse LogEntry verweisen
    /// </summary>
    public class UsagePointAdapterTRuDI : UsagePoint
    {
        public UsagePointAdapterTRuDI()
        {
            this.MeterReadings = new List<MeterReading>();
            this.LogEntries = new List<LogEntry>();
        }

        /// <summary>
        /// Diese Objekte sind erforderlich. Ein UsagePointAdapterTRuDI muss auf mindestens ein Objekt
        /// der Klasse MeterReading referenzieren
        /// </summary>
        public List<MeterReading> MeterReadings
        {
            get; set;
        }

        /// <summary>
        /// Die Logdaten sind optional
        /// </summary>
        public List<LogEntry> LogEntries
        {
            get; set;
        }
    }
}
