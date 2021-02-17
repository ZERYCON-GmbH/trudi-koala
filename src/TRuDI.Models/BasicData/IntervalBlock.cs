namespace TRuDI.Models.BasicData
{
    using System.Collections.Generic;

    /// <summary>
    /// Die Klasse IntervalBlock enthält die einzelnen Intervallwerte. Eine Nachricht 
    /// muss mindestens eine Instanz der Klasse IntervalBlock enthalten.
    /// 
    /// Jede Instanz der Klasse IntervalBlock:
    /// muss auf mindestens eine Instanz der Klasse IntervalReading verweisen.
    /// </summary>
    public class IntervalBlock
    {
        /// <summary>
        /// Im Konstruktor wird sichergestellt, dass alle vorhandenen Listen 
        /// vor dem ersten Zugriff initialisert werden.
        /// </summary>
        public IntervalBlock()
        {
            this.IntervalReadings = new List<IntervalReading>();
        }

        /// <summary>
        /// Der Verweis auf die Instanz der Klasse MeterReading, zu der 
        /// die Instanz der Klasse IntervalBlock zugehörig ist.
        /// </summary>
        public MeterReading MeterReading
        {
            get; set;
        }

      
        /// <summary>
        /// Jede Instanz Der Klasse IntervalReading beinhaltet die Daten
        /// zu einem konkreten Messwert.
        /// </summary>
        public List<IntervalReading> IntervalReadings
        {
            get; set;
        }

        /// <summary>
        /// Das Datenelement interval beschreibt die gesamte Zeitperiode, für 
        /// die die nachfolgenden Messwerte in der Messwertliste enthalten sind.
        /// 
        /// Die Zeitperiode wird durch einen Startzeitpunkt und eine Dauer definiert.
        /// 
        /// Der Startzeitpunkt wird als xs:dateTime beschrieben. Die Dauer wird als ganzzahliger
        /// Sekundenwert beschrieben.
        /// 
        /// Jede Instanz der Klasse IntervalBlock muss ein Datenelement vom Typ interval enthalten.
        /// </summary>
        public Interval Interval
        {
            get; set;
        }

    }
}
