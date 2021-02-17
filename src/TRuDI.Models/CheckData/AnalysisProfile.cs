namespace TRuDI.Models.CheckData
{
    using System.Collections.Generic;

    using TRuDI.HanAdapter.Interface;
    using TRuDI.Models.BasicData;

    /// <summary>
    /// Die Klasse AnalysisProfile repräsentiert das Auswerteprofil. Bei der Übertragung von 
    /// Daten zu Zwecken der eichrechtskonformen Rechnungsüberprüfung muss die Nachricht eine 
    /// Instanz der Klasse enthalten.Bei der Übertragung von Daten für das Alltagsdisplay ist 
    /// die Nutzung der Klasse AnalysisProfile optional.
    /// 
    /// Eine Instanz der Klasse AnalysisProfile:
    ///	 
    ///     - muss auf mindestens eine Instanz der Klasse TariffStage verweisen
    /// 
    /// </summary>
    public class AnalysisProfile
    {
        /// <summary>
        /// Im Konstruktor wird sichergestellt, dass alle vorhandenen Listen 
        /// vor dem ersten Zugriff initialisert werden.
        /// </summary>
        public AnalysisProfile()
        {
            this.TariffStages = new List<TariffStage>();
        }

        /// <summary>
        /// Die Klasse TariffStage spezifiziert einzelne Tarifstufen des Auswerteprofils.
        /// Ein Auswerteprofil muss mindestens eine Instanz der Klasse TariffStage enthalten.
        /// </summary>
        public List<TariffStage> TariffStages
        {
            get; set;
        }

        /// <summary>
        /// Das Datenelement billingPeriod spezifiziert den Abrechnungszeitraum für den dieses 
        /// Auswerteprofil Gültigkeit hat.
        /// 
        /// Eine Instanz der Klasse AnalysisProfile muss genau ein Datenelement billingPeriod enthalten.
        /// </summary>
        public Interval BillingPeriod
        {
            get; set;
        }

        /// <summary>
        /// Die Klasse TariffChangeTrigger abstrahiert alle weiteren Triggerformen 
        /// und damit alle weiteren Tarifumschaltgründe. Eine Instanz der Klasse ist optional.
        /// </summary>
        public TariffChangeTrigger TariffChangeTrigger
        {
            get; set;
        }


        /// <summary>
        /// Das Datenelement tariffUseCase spezifiziert den Tarifanwendungsfall für den diese Tarifstufe gültig ist. 
        /// Die angegebene Nummer entspricht dabei dem Anwendungsfall wie er in der TR-03109-1 des BSI definiert wurde.
        /// Eine Instanz der Klasse AnalysisProfile muss genau ein Datenelement der Klasse tariffUseCase beinhalten.
        /// </summary>
        public TafId TariffUseCase
        {
            get; set;
        }


        /// <summary>
        /// Das Datenelement tariffId entspricht der Tarif-kennbezeichnung des Lieferanten.
        /// Eine Instanz der Klasse AnalysisProfile muss genau ein Datenelement tariffId enthalten.
        /// </summary>
        public string TariffId
        {
            get; set;
        }


        /// <summary>
        /// Das Datenelement defaultTariffNumber ist eine Referenz auf die Standardtarifstufe, 
        /// die zu Beginn eines Abrechnungszeitraums gültig ist.
        /// 
        /// Eine Instanz der Klasse AnalysisProfile muss genau ein Datenelement defaultTariffNumber enthalten.
        /// </summary>
        public ushort DefaultTariffNumber
        {
            get; set;
        }

    }
}
