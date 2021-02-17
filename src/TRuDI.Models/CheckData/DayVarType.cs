namespace TRuDI.Models.CheckData
{
    using System.Diagnostics;

    /// <summary>
    /// Die Klasse DayVarType kann genutzt werden, um Datumsangaben darzustellen, 
    /// zum Beispiel um Feiertage zu beschreiben. Ist in der Klasse DayVarType keine 
    /// Jahresangabe enthalten, so gilt das angegebene Datum jährlich. 
    /// Sind keine Monatsangaben enthalten, so gilt das angegebene Datum monatlich.
    /// 
	/// Die Klasse DayVarType verweist auf keine weiteren Klassen.
    ///
    /// </summary>
    [DebuggerDisplay("{Year}-{Month}-{DayOfMonth")]
    public class DayVarType
    {
        /// <summary>
        /// Das Datenelement dayOfMonth kann genutzt werden, um den genauen Tag 
        /// innerhalb eines Monats zu spezifizieren. 
        /// Die Nutzung des Datenelements dayOfMonth ist optional.
        /// </summary>
        public byte? DayOfMonth 
        {
            get; set;
        }

        /// <summary>
        /// Das Datenelement month kann genutzt werden um den Monat als ganzzahligen Wert 
        /// zu beschreiben. Dabei ist 1 = Januar und 12 = Dezember, die weiteren Monate 
        /// entsprechend kalendarischer Reihenfolge.
        /// Die Nutzung des Datenelements month ist optional.
        /// </summary>
        public byte? Month 
        {
            get; set;
        }

        /// <summary>
        /// Das Datenelement year kann genutzt werden, um ein bestimmtes Jahr als ganzzahligen Wert
        /// zu beschreiben. 
        /// 
        /// Die Nutzung des Datenelements year ist optional.
        /// </summary>
        public ushort? Year
        { 
            get; set;
        }

        /// <summary>
        /// Spezifiziert das Datum als jährlich, wenn Year nicht belegt ist
        /// </summary>
        public bool Yearly
        {
            get; set;
        }

        /// <summary>
        /// Spezifiziert das Datum als monatlich, wenn Month nicht belegt ist
        /// </summary>
        public bool Monthly
        {
            get; set;
        }
    }
}
