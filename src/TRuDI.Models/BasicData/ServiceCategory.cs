namespace TRuDI.Models.BasicData
{
    /// <summary>
    /// Die Klasse ServiceCategory repräsentiert nach ESPI REQ.21 die Sparte des Produkts (Service), 
    /// welches dem Letztverbraucher am Zählpunkt zur Verfügung gestellt wird.
    /// 
    /// Die Klasse ServiceCategory enthält keine Verweise auf weitere Klassen.
    /// </summary>
    public class ServiceCategory
    {
        /// <summary>
        /// kind beschreibt als Datenelement die konkrete Sparte des Zählpunktes.
        /// Gültige Werte nach ESPI REQ.21 sind:
        ///     0 – electricity(Elektrizität)
        ///     1 – gas(Gas)
        ///     2 – water(Wasser)
        ///     4 – pressure(Druck)
        ///     5 – heat(Wärme)
        ///     6 – cold(Kälte)
        ///     7 – communication(Kommunikation)
        ///     8 – time(Zeit)
        ///     
        ///Eine Instanz der Klasse ServiceCategory muss genau einen der genannten Werte für das Datenelement kind beinhalten.
        /// </summary>
        public Kind? Kind
        {
            get; set;
        }
    }
}
