namespace TRuDI.Models.BasicData
{
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// Die Klasse MeterReading repräsentiert den Kopf einer Messwertliste.
    /// Die Klasse enthält untergeordnet die entsprechenden Werte und Zusatzinformationen 
    /// einer Messwertliste. 
    /// 
    /// Eine Instanz der Klasse MeterReading:
	///     - Muss auf eine Instanz der Klasse ReadingType verweisen
	///     - Muss auf mindestens eine Instanz der Klasse IntervalBlock verweisen
    /// 	- Muss auf mindestens eine Instanz der Klasse Meter verweisen
    /// 	
    /// </summary>
    public class MeterReading
    {
        /// <summary>
        /// Im Konstruktor wird sichergestellt, dass alle vorhandenen Listen 
        /// vor dem ersten Zugriff initialisert werden.
        /// </summary>
        public MeterReading()
        {
            this.IntervalBlocks = new List<IntervalBlock>();
            this.Meters = new List<Meter>();
        }

        public UsagePoint UsagePoint
        {
            get; set;
        }

        /// <summary>
        /// Die Klasse ReadingType spezifiziert die Inhalte einer Messwertliste.
        /// Jede Messwertliste muss eine Instanz der Klasse ReadingType beinhalten.
        /// </summary>
        public ReadingType ReadingType
        {
            get; set;
        }

        /// <summary>
        /// Die Klasse IntervalBlock enthält die einzelnen Intervallwerte. Eine Nachricht 
        /// muss mindestens eine Instanz der Klasse IntervalBlock enthalten.
        /// </summary>
        public List<IntervalBlock> IntervalBlocks
        {
            get; set;
        }

        /// <summary>
        /// Die Klasse Meter identifiziert den Zähler über die enthal-tene Zählerkennung. 
        /// Eine originäre Messliste muss eine Zählernummer enthalten.
        /// </summary>
        public List<Meter> Meters
        {
            get; set;
        }

        /// <summary>
        /// Die MeterReadingId identifiziert eine Messwertliste eindeutig. Die Id kann
        /// zum Beispiel aus der Zählpunktbezeichnung, der Gerätenummer und der 
        /// OBIS-Kennziffer zusammengesetzt werden. 
        /// 
        /// Eine Instanz der Klasse MeterReading muss ein Datenelement vom Typ
        /// meterReadingId enthalten.
        /// </summary>
        public string MeterReadingId
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance uses the TargetTime property.
        /// </summary>
        public bool IsTargetTimeUsed { get; set; }
    }
}
