namespace TRuDI.Models.CheckData
{
    using System.Collections.Generic;

    /// <summary>
    /// Die Klasse TimeTrigger beschreibt einen Trigger, 
    /// der anhand von zeitlichen Angaben einen Tarifstufenwechsel auslöst.
    /// 
    /// Eine Instanz der Klasse TimeTrigger:
    /// 
	///     - muss auf mindestens eine Instanz der Klasse DayProfile verweisen
	///     - kann auf Instanzen der Klasse SpecialDayProfile verweisen
    ///
    /// </summary>
    public class TimeTrigger
    {
        /// <summary>
        /// Im Konstruktor wird sichergestellt, dass alle vorhandenen Listen 
        /// vor dem ersten Zugriff initialisert werden.
        /// </summary>
        public TimeTrigger()
        {
            this.DayProfiles = new List<DayProfile>();
            this.SpecialDayProfiles = new List<SpecialDayProfile>();
        }

        /// <summary>
        /// Instanzen der Klasse DayProfile kapseln ein oder mehrere Instanzen 
        /// der Klasse DayTimeProfiles.
        /// </summary>
        public List<DayProfile> DayProfiles
        {
            get; set;
        }

        /// <summary>
        /// Die Klasse SpecialDayProfile spezifiziert Sondertage, 
        /// die ein bestimmtes Tagesprofil abbilden müssen.
        /// </summary>
        public List<SpecialDayProfile> SpecialDayProfiles
        {
            get; set;
        }
    }
}
