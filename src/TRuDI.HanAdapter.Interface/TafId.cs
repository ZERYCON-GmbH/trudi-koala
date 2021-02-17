namespace TRuDI.HanAdapter.Interface
{
    /// <summary>
    /// Enumeration of TAF-IDs.
    /// </summary>
    public enum TafId
    {
        /// <summary>
        /// Datensparsamer Tarif
        /// </summary>
        Taf1 = 1,

        /// <summary>
        /// Zeitvariabler Tarif
        /// </summary>
        Taf2 = 2,

        /// <summary>
        /// Lastvariable Tarife
        /// </summary>
        Taf3 = 3,

        /// <summary>
        /// Verbrauchsvariable Tarife
        /// </summary>
        Taf4 = 4,

        /// <summary>
        /// Ereignisvariable Tarife
        /// </summary>
        Taf5 = 5,

        /// <summary>
        /// Abruf von Messwerten im Bedarfsfall
        /// </summary>
        Taf6 = 6,

        /// <summary>
        /// Z�hlerstandsgangsmessung
        /// </summary>
        Taf7 = 7,

        /// <summary>
        /// Erfassung von Extremwerten.
        /// </summary>
        Taf8 = 8,

        /// <summary>
        /// Abruf der Ist-Einspeisung einer Erzeugungsanlage.
        /// </summary>
        Taf9 = 9,

        /// <summary>
        /// Abruf von Netzzustandsdaten
        /// </summary>
        Taf10 = 10,

        /// <summary>
        /// Steuerung von unterbrechbaren Verbrauchseinrichtungen und Erzeugungsanlagen
        /// </summary>
        Taf11 = 11,

        /// <summary>
        /// Prepaid Tarif 
        /// </summary>
        Taf12 = 12,

        /// <summary>
        /// Bereitstellung von Messwerts�tzen zur Visualisierung f�r den Letztverbraucher �ber die WAN-Schnittstelle
        /// </summary>
        Taf13 = 13,

        /// <summary>
        /// Hochfrequente Messwertbereitstellung f�r Mehrwertdienste
        /// </summary>
        Taf14 = 14
    }
}