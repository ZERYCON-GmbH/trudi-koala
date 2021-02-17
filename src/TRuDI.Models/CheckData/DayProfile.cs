namespace TRuDI.Models.CheckData
{
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Instanzen der Klasse DayProfile kapseln ein oder mehrere Instanzen der Klasse DayTimeProfiles.
    /// 
    /// Eine Instanz der Klasse muss auf mindestens eine Instanz der Klasse DayTimeProfiles verweisen.
    /// </summary>
    [DebuggerDisplay("DayId={DayId}")]
    public class DayProfile
    {
        /// <summary>
        /// Im Konstruktor wird sichergestellt, dass alle vorhandenen Listen 
        /// vor dem ersten Zugriff initialisert werden.
        /// </summary>
        public DayProfile()
        {
            this.DayTimeProfiles = new List<DayTimeProfile>();
        }

        /// <summary>
        /// Die Klasse DayTimeProfile spezifiziert die Tageszeit für die übergeordnete 
        /// Day Profile Instanz
        /// </summary>
        public List<DayTimeProfile> DayTimeProfiles
        {
            get; set;
        }

        /// <summary>
        /// Das Datenelement DayId identifiziert ein Tagesprofil eindeutig. 
        /// Es wird von anderen Klassen genutzt, um auf das entsprechende 
        /// Tagesprofil zu referenzieren.
        /// 
        /// Jede Instanz der Klasse dayProfile muss ein Datenelement vom Typ 
        /// dayId beinhalten.
        /// </summary>
        public ushort? DayId 
        {
            get; set;
        }
        
    }
}
