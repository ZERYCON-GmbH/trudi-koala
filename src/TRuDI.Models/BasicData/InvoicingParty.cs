namespace TRuDI.Models.BasicData
{
    /// <summary>
    /// Die Klasse InvoicingParty repräsentiert den Rechnungssteller bzw. die Marktrolle, die für die Tarifierung
    /// der Messdaten für den Letztverbraucher verantwortlich ist. Jede Nachricht muss eine Instanz der Klasse 
    /// InvoicingParty beinhalten.
    /// </summary>
    public class InvoicingParty
    {
        /// <summary>
        /// Das Datenelement invoicingPartyId wird genutzt, um die eindeutige Kennung des Marktteilnehmers, 
        /// welcher für die Tarifierung der Messdaten für den Letztverbraucher verantwortlich ist. Dies kann 
        /// zum Beispiel die Kennung des Logical Devices des EMT im SMGW ohne die En-dung „.sm“ sein. Die 
        /// eindeutige Kennung muss dem Letztverbraucher vorab bekannt gemacht werden. 
        /// Jede Instanz der Klasse InvoicingParty muss ein Datenelement vom Typ invoicingPartyId beinhalten.
        /// </summary>
        public string InvoicingPartyId
        {
            get; set;
        }

    }
}
