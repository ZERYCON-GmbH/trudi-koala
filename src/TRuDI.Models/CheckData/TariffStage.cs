namespace TRuDI.Models.CheckData
{
    using System.Diagnostics;

    /// <summary>
    /// Die Klasse TariffStage spezifiziert einzelne Tarifstufen 
    /// des Auswerteprofils. Ein Auswerteprofil muss mindestens 
    /// eine Instanz der Klasse TariffStage enthalten.
    /// </summary>
    [DebuggerDisplay("{ObisCode} - {TariffNumber} ({Description})")]
    public class TariffStage
    {
        /// <summary>
        /// Das Datenelement tariffNumber spezifiziert die Tarifnummer der Tarifstufe. 
        /// Eine Instanz der Klasse TariffStage muss genau ein Datenelement tariffNumber enthalten.
        /// </summary>
        public ushort TariffNumber
        {
            get; set;
        }

        /// <summary>
        /// Das Datenelement description dient zur freien Beschreibung der Tarifstufe. 
        /// Die Nutzung ist optional.
        /// </summary>
        public string Description
        {
            get; set;
        }

        /// <summary>
        /// Das Datenelement obisCode der Klasse TariffStage beschreibt den OBISCode der Tarifstufe. 
        /// Mit Hilfe des OBIS-Codes ist wird eine Zuordnung der Tarifstufen zu Messwertlisten ermöglicht.
        /// Dastellung als HexCode ohne Trennzeichen im einem String-Datenformat Jede Instanz der Klasse 
        /// TariffStage muss genau ein Datenelement obis-Code enthalten.
        /// </summary>
        public string ObisCode
        {
            get; set;
        }
    }
}
