namespace TRuDI.Models.BasicData
{
    /// <summary>
    /// Die Enumeration stellt die Kodierung von eichrechtlich releveanten Fehlern 
    /// laut PTB 50.8 dar
    /// StatusPTB wird in IntervalReading benötigt.
    /// </summary>
    public enum StatusPTB : byte
    {
        /// <summary>
        /// Kein Fehler
        /// </summary>
        NoError = 0,

        /// <summary>
        /// Warnung, keine (eichrechtliche) Aktion notwendig, Messwert gültig.
        /// </summary>
        Warning = 1,

        /// <summary>
        /// Temporärer Fehler, gesendeter Messwert wird als ungültig gekennzeichnet, 
        /// der Wert im Messwertfeld kann entsprechend den Regeln [VDE4400] 
        /// bzw. [G685] im Backend als Ersatzwert verwendet werden.
        /// </summary>
        TemporaryError = 2,

        /// <summary>
        /// Temporärer Fehler, gesendeter Messwert ist ungültig, der im Messwertfeld 
        /// enthaltene Wert kann im Backend nicht als Ersatzwert verwendet werden.
        /// </summary>
        CriticalTemporaryError = 3,

        /// <summary>
        /// Fataler Fehler (Zähler defekt), der aktuell gesendete und alle 
        /// zukünftigen Messwerte sind ungültig.
        /// </summary>
        FatalError = 4
    }
}
