namespace TRuDI.Models.BasicData
{
    /// <summary>
    /// Das Datenelement level beschreibt die dem Ereignis zugeordnete Rubrik 
    /// als ein Element der Enumeration aus
    ///     1 - Info
    ///     2 - Warning
    ///     3 - Error
    ///     4 - Fatal
    ///     5 - Extension
    /// 
    /// Das Datenelement ist optional anzugeben
    /// Es wird in der Klasse LogEvent verwendet.
    /// </summary>
    public enum Level : byte
    {
        INFO = 1,
        WARNING,
        ERROR,
        FATAL,
        EXTENSION
    }
}
