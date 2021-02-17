namespace TRuDI.Models.BasicData
{
    /// <summary>
    /// Kind beschreibt die konkrete Sparte des Zählpunktes nach ESPI REQ.21.
    /// </summary>
    public enum Kind : ushort
    {
        Electricity = 0,
        Gas = 1,
        Water = 2,
        Pressure = 4,
        Heat = 5,
        Cold = 6,
        Communication = 7,
        Time = 8
    }
}
