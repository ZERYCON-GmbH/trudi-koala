namespace TRuDI.Models.BasicData
{
    /// <summary>
    /// Das Datenelement powerOfTenMultiplier repräsentiert den Einheitenvorsatz der Maßeinheit der in der Messwertliste übermittelten Werte.
    /// Gültige Werte nach ESPI REQ.21 sind:
    /// 
    ///         0 = None
    ///         1 = deca= x10
    ///         2 = hecto= x100
    ///        –3 = mili= x10–3
    ///         3 = kilo= x1000
    ///         6 = Mega= x106
    ///        –6 = micro= x10–3
    ///         9 = Giga= x109
    ///         
    /// Die Enumeration powerofTenMultiplier wird in der Klasse ReadingType benötigt.
    /// </summary>
    public enum PowerOfTenMultiplier : short
    {
        micro = -6,
        mili = -3,
        None = 0,
        deca = 1,
        hecto = 2,
        kilo = 3,
        Mega = 6,
        Giga = 9
    }
}
