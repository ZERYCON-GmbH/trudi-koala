namespace TRuDI.Models.BasicData
{
    /// <summary>
    /// Das Datenelement outcome spezifiziert das Ergebnis der mit dem Ergebnis
    /// verbundenen Atkion. 
    ///     
    ///     0 - SUCCESS
    ///     1 - FAILURE
    ///     2 - EXTENSION
    ///     
    /// Das Datenelement ist optional anzugeben.
    /// Es wird in der Klasse LogEvent verwendet.
    /// </summary>
    public enum Outcome : byte
    {
        SUCCESS = 0,
        FAILURE,
        EXTENSION 
    }
}
