namespace TRuDI.Models.CheckData
{
    /// <summary>
    /// Die Klasse TimeVarType kann genutzt werden um feste (zeitzonenunabhängige) 
    /// Zeitangaben als Einzelwerte mit Stunden- und Minutenangabe zu definieren.
    /// Die Klasse TimeVarType verweist auf keine weite-ren Klassen.
    /// </summary>
    public class TimeVarType
    {

        /// <summary>
        /// Wenn Die Werte nicht gesetzt sind, wird bei jedem Wert "0" als gesetzt gesehen.
        /// Daher wird im Konstruktor jedes Element per Default mit "0" belegt.
        /// </summary>
        public TimeVarType()
        {
            this.Hour = 0;
            this.Minute = 0;
            this.Second = 0;
            this.Hundreds = 0;
        }
        
        /// <summary>
        /// Das Datenelement hour der Klasse TimeVarType kann genutzt werden um eine ganzzahlige Stun-denangabe zu definieren.
        /// Die Nutzung des Datenelements hour ist optional.
        /// Wenn das Datenelement nicht in einer Instanz der Klasse TimeVarType vorhanden ist, 
        /// so wird der Wert „0“ als gesetzt angesehen.
        /// </summary>
        public byte? Hour
        {
            get; set;
        }

        /// <summary>
        /// Das Datenelement minute der Klasse TimeVarType kann genutzt werden um eine ganzzahlige Minutenangabe zu definieren.
        /// Die Nutzung des Datenelements minute ist optional.
        /// Wenn das Datenelement nicht in einer Instanz der Klasse TimeVarType vorhanden ist, 
        /// so wird der Wert „0“ als gesetzt angesehen.
        /// </summary>
        public byte? Minute
        {
            get; set;
        }

        /// <summary>
        /// Das Datenelement second der Klasse TimeVarType kann genutzt werden um eine ganzzahlige Sekundenangabe zu definieren.
        /// Die Nutzung des Datenelements second ist optional. Wenn das Datenelement nicht in einer Instanz der Klasse 
        /// TimeVarType vorhanden ist, so wird der Wert „0“ als gesetzt angesehen.
        /// </summary>
        public byte? Second
        {
            get; set;
        }

        /// <summary>
        /// Das Datenelement hundreds der Klasse TimeVarType kann genutzt werden um eine ganzzahlige 
        /// HunderstelSekundenangabe zu definieren.
        /// Die Nutzung des Datenelements hundreds ist optional. Wenn das Datenelement nicht in einer 
        /// Instanz der Klasse TimeVarType vorhanden ist, so wird der Wert „0“ als gesetzt angesehen.
        /// </summary>
        public byte? Hundreds
        {
            get; set;
        }

    }
}
