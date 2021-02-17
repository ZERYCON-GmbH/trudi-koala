namespace TRuDI.Models.BasicData
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Die Klasse LogEvent spezifiziert Logereignisse.
    /// Instanzen der Klasse sind optional.
    /// </summary>
    [DebuggerDisplay("{Timestamp}, {Level}, {Outcome}, {Text}")]
    public class LogEvent
    {

        /// <summary>
        /// Das Datenelement level beschreibt die dem Ereignis zugeordnete Rubrik 
        /// als ein Element der Enumeration aus
        ///     1 - Info
        ///     2 - Warning
        ///     3 - Error
        ///     4 - Fatal
        ///     5 - Extension
        /// 
        /// Das Datenelement ist optional anzugeben
        /// </summary>
        public Level Level 
        {
            get; set;
        }

        /// <summary>
        /// Das Datenelement text liefert die textuelle Beschreibung des Logeintrags.
        /// 
        /// Das Datenelement ist optional anzugeben.
        /// </summary>
        public string Text
        {
            get; set;
        }

        /// <summary>
        /// Das Datenelement outcome spezifiziert das Ergebnis der mit dem Ergebnis
        /// verbundenen Atkion. 
        ///     
        ///     0 - SUCCESS
        ///     1 - FAILURE
        ///     2 - EXTENSION
        ///     
        /// Das Datenelement ist optional anzugeben.
        /// </summary>
        public Outcome? Outcome 
        {
            get; set;
        }

        /// <summary>
        /// Das Datenelement timestamp beschreibt den Zeitstempel mit dem Zeitpunkt,
        /// wann das Ereignis eingetreten ist.
        /// 
        /// Das Datenelement ist optional anzugeben.
        /// </summary>
        public DateTime Timestamp
        {
            get; set;
        }

    }
}
