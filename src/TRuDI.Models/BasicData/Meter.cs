namespace TRuDI.Models.BasicData
{
    /// <summary>
    /// Die Klasse Meter identifiziert den Zähler über die enthal-tene Zählerkennung. 
    /// Eine originäre Messliste muss eine Zählernummer enthalten.
    /// 
    /// Die Klasse Meter verweist auf keine weiteren Klassen
    /// </summary>
    public class Meter
    {
        /// <summary>
        /// Das Datenfeld meterId repräsentiert die Zählernummer des Smart Meters.
        /// Eine Instanz der Klasse muss ein Datenelement meterId enthalten.
        /// </summary>
        public string MeterId
        {
            get; set;
        }
    }
}
