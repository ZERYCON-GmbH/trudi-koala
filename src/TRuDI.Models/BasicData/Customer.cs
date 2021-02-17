namespace TRuDI.Models.BasicData
{
    /// <summary>
    /// Die Klasse Customer repräsentiert den Letztverbraucher, welcher über eine je
    /// Rechnungsstelllende eindeuige Kennung beschrieben wird. Jede Nachricht an der 
    /// Schnittstelle IF_Adapter_TRuDI muss eine Instanz der Klasse Customer beinhalten.
    /// 
    /// Die Klasse Customer verweist auf keine weiteren Klassen.
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// Das Datenelement customerId beinhaltet eine eindeutige Identifikation des 
        /// Letztverbrauchers. Diese Identifikation sollte mindesetens je Marktteilnehmer
        /// eindeutig sein. Genutzt werden kann hier die Kennung des Logical Devices des 
        /// Letztverbrauchers im SMGW ohne die Endung "sm".
        /// 
        /// Jede Instanz der Klasse Customer muss ein Datenelement vom Typ customerId beinhalten.
        /// </summary>
        public string CustomerId
        {
            get; set;
        }  
    }
}
