namespace TRuDI.Models.CheckData
{
    using System.Diagnostics;

    /// <summary>
    /// Die Klasse DayTimeProfile spezifiziert die Tageszeit für die übergeordnete DayProfile-Instanz
    /// </summary>
    [DebuggerDisplay("{StartTime.Hour}:{StartTime.Minute}:{StartTime.Second}, Tariff={TariffNumber}")]
    public class DayTimeProfile
    {
        /// <summary>
        /// Im Konstruktur wird die Startzeit per Default auf 0:0:0:0 gesetzt
        /// </summary>
        public DayTimeProfile()
        {
            this.StartTime = new TimeVarType { Hour = 0, Minute = 0, Second = 0, Hundreds = 0 };
        }

        /// <summary>
        /// Verweis auf die übergeordnete DayProfile Instanz
        /// </summary>
        public DayProfile DayProfile
        {
            get; set;
        }

        /// <summary>
        /// Das Datenelement startTime spezifiziert den Startzeitpunkt des referenzierenden Tagesprofiles 
        /// (Klasse DayProfile). Der Startzeitpunkt wird aus der (zeitzonenunabhängigen) Angabe der Stunde 
        /// (TimeVarType – hour) und der Minuten (TimeVar-Type – minute) gebildet. 
        /// Als Default-Wert sollten für beide Werte 0 genutzt werden.
        /// 
        /// Jede Instanz der Klasse DayTimeProfile muss ein Datenelement vom Typ startTime enthalten.
        /// </summary>
        public TimeVarType StartTime 
        {
            get; set;
        }

        /// <summary>
        /// Das Datenelement tariffNumber verweist auf die Tarifstufe, die zu der angegebenen Startzeit 
        /// (Datenelement startTime) gültig wird. Das Datenelement muss auf eine Instanz des Datenelements 
        /// tariffNumber in der Klasse TariffStage verweisen.
        /// 
        /// Jede Instanz der Klasse DayTimeProfile muss ein Datenelement vom Typ tariffNumber enthalten.
        /// </summary>
        public ushort? TariffNumber
        {
            get; set;
        }

    }
}
