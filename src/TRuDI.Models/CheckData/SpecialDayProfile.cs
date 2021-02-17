namespace TRuDI.Models.CheckData
{
    using System.Diagnostics;

    /// <summary>
    /// Die Klasse SpecialDayProfile spezifiziert Sondertage, 
    /// die ein bestimmtes Tagesprofil abbilden müssen.
    /// Instanzen der Klasse SpecialDayProfile sind optional.
    ///
    /// Eine Instanz der Klasse SpecialDayProfile:
	///     
    ///     - muss auf eine Instanz der Klasse DayProfile verweisen.
    /// </summary>
    [DebuggerDisplay("Id={DayId}, Date={SpecialDayDate.Year}-{SpecialDayDate.Month}-{SpecialDayDate.DayOfMonth}")]
    public class SpecialDayProfile
    {
        /// <summary>
        /// Instanzen der Klasse DayProfile kapseln ein oder mehrere 
        /// Instanzen der Klasse DayTimeProfiles.
        /// </summary>
        public DayProfile DayProfile
        {
            get; set;
        }
      
        /// <summary>
        /// Das Datenelement dayId der Klasse SpecialDayProfile referenziert auf das entsprechende 
        /// Tagesprofil, welches die Startzeit definiert.
        /// Jede Instanz der Klasse SpecialDayProfile muss ein Datenelement vom Typ dayId enthalten.
        /// </summary>
        public ushort DayId
        {
            get; set;
        }

        /// <summary>
        /// Das Datenelement specialDayDate spezifiziert das Datum des Sondertages. 
        /// Das Datum wird über die Klasse DayVarType beschrieben.
        /// Eine Instanz der Klasse SpecialDayProfile muss ein Datenelement vom 
        /// Typ specialDayDate enthalten.
        /// </summary>
        public DayVarType SpecialDayDate
        {
            get; set;
        }
    }
}
