namespace TRuDI.Models.BasicData
{
    using System.Collections.Generic;

    using TRuDI.Models.CheckData;

    /// <summary>
    /// Die Klasse UsagePoint repräsentiert den Zählpunkt und stellt das zentrale Datenelement 
    /// einer Nachricht dar.Jede Nachricht muss mindestens einen Zählpunkt beinhalten.
    /// 
    /// Eine Instanz der Klasse UsagePoint:
    ///	    muss auf eine Instanz der Klasse InvoicingParty verweisen
    ///	    IF_Adapter_TRuDI: Usagepoint muss auf eine Instanz der Klasse Customer verweisen
    ///	    IF_Lieferant_TRuDI: Usagepoint kann auf eine Instanz der Klasse Customer verweisen
    ///	    
    ///	    muss auf eine Instanz der Klasse SMGW verweisen
    ///	    
    ///	    muss auf eine Instanz der Klasse ServiceCategory verweisen
    ///	    
    ///	    muss auf mindestens eine Instanz der Klasse MeterReading verweisen (IF_Adapter_TRuDI)
    ///	    
    ///	    kann auf Instanzen der Klasse LogEntry verweisen (IF_Adapter_TRuDI)
    ///	    
    ///	    kann auf eine Instanz der Klasse AnalysisProfile verweisen (IF_Lieferant_TRuDI)
    /// </summary>
    public abstract class UsagePoint
    {
        /// <summary>
        /// Im Konstruktor wird sichergestellt, dass alle vorhandenen Listen 
        /// vor dem ersten Zugriff initialisert werden.
        /// </summary>
        public UsagePoint()
        {
            this.Certificates = new List<Certificate>();
        }

        /// <summary>
        /// TRuDI-Version bzw. Version des Programmes, welches die Datei erzeugt hat.
        /// </summary>
        public string GeneratorVersion { get; set; }

        /// <summary>
        /// Die Klasse InvoicingParty repräsentiert den Rechnungssteller bzw. die Marktrolle, die für die Tarifierung
        /// der Messdaten für den Letztverbraucher verantwortlich ist. 
        /// 
        /// Jede Nachricht muss eine Instanz der Klasse InvoicingParty beinhalten.
        /// </summary>
        public InvoicingParty InvoicingParty
        {
            get; set;
        }

        /// <summary>
        /// Die Klasse Customer repräsentiert den Letztverbraucher, welcher über eine je
        /// Rechnungsstelllende eindeuige Kennung beschrieben wird. Jede Nachricht an der 
        /// Schnittstelle IF_Adapter_TRuDI muss eine Instanz der Klasse Customer beinhalten.
        /// </summary>
        public Customer Customer
        {
            get; set;
        }

        /// <summary>
        /// Die Klasse SMGW repräsentiert Informationen zum Smart Meter Gateway, 
        /// von welchem die beinhaltenden Messwertlisten stammen. 
        /// 
        /// Eine Nachricht muss eine Instanz von SMGW enthalten.
        /// </summary>
        public SMGW Smgw
        {
            get; set;
        }

        /// <summary>
        /// Die Klasse ServiceCategory repräsentiert nach ESPI REQ.21 die Sparte des Produkts (Service), 
        /// welches dem Letztverbraucher am Zählpunkt zur Verfügung gestellt wird.
        /// </summary>
        public ServiceCategory ServiceCategory
        {
            get; set;
        }

        /// <summary>
        /// Die Klasse Certificate repärsentiert das Zertifikat, welches für die Inhalstdatensignierung (WAN SIG),
        /// für die TSL-Verschlüsselung am HAN oder für die Signierung von Zertifikaten durch eine SubCA genutzt 
        /// wird. Wird von einer anderen Rolle als dem SMGW ein Messwert signiert (z.B. bei manueller Änderung 
        /// eines Messwerts), so ist das entsprechende Zertifikat hier zusätzlich einzufügen. 
        /// 
        /// Für eine eichrechtlich-konforme Überprüfung der Daten muss die Nachricht mindestens eine Instanz der 
        /// Klasse Certificate beinhalten.
        /// </summary>
        public List<Certificate> Certificates
        {
            get; set;
        }

        /// <summary>
        /// Die usagePointId entspricht der Zählpunktbezeichnung nach dem aktuellen MeteringCode. 
        /// Eine Instanz der Klasse UsagePoint muss genau eine Zählpunktbezeichnung enthalten. 
        /// </summary>
        public string UsagePointId
        {
            get; set;
        }

        /// <summary>
        /// Das Datenelement tariffName beinhaltet eine Identifikation des Tarifs.
        /// Jede Instanz der Klasse UsagePoint muss ein Datenelement vom Typ tariffName beinhalten.
        /// </summary>
        public string TariffName
        {
            get; set;
        }

        /// <summary>
        /// Die Klasse AnalysisProfile repräsentiert das Aus-werteprofil. Bei der Übertragung von
        /// Daten zu Zwecken der eichrechtskonformen Rechnungs-überprüfung muss die Nachricht eine 
        /// Instanz der Klasse enthalten. Bei der Übertragung von Daten für das Alltagsdisplay 
        /// ist die Nutzung der Klasse AnalysisProfile optional.
        /// </summary>
        public AnalysisProfile AnalysisProfile
        {
            get; set;
        }
    }
}
