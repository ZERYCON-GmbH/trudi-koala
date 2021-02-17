namespace TRuDI.Models.BasicData
{
    using System.Collections.Generic;

    /// <summary>
    /// Die Klasse SMGW repräsentiert Informationen zum Smart Meter Gateway, 
    /// von welchem die beinhaltenden Messwertlisten stammen. 
    /// 
    /// Eine Nachricht muss eine Instanz von SMGW enthalten.
    /// 
    /// Die Klasse SMGW verweist auf keine weiteren Klassen.
    /// </summary>
    public class SMGW
    {
        /// <summary>
        /// Im Konstruktor wird sichergestellt, dass alle vorhandenen Listen 
        /// vor dem ersten Zugriff initialisert werden.
        /// </summary>
        public SMGW()
        {
        }

        /// <summary>
        /// Das Datenelement smgwId repräsentiert die Kennung des Smart Meter Gateways, von denen die Daten 
        /// der Nachricht und der öffentliche Schlüssel stammen.
        /// 
        /// Jede Instanz der Klasse SMGW muss ein Datenelement vom Typ smgwId enthalten.
        /// 
        /// </summary>
        public string SmgwId
        {
            get; set;
        }

        /// <summary>
        /// Das Datenelement certId enthält die ID eines Zertifikats, das inhaltlich zu diesem SMGW gehört. 
        /// Das Zertifikat selbst und um welchen Typ (HAN Zertifikat des SMGW, WAN SIG Zertifikat des SMGW, 
        /// SubCA Zertifikat der ausstellen-den SubCA) wird in der Klasse Certificate übertragen. 
        /// Jede Instanz von SMGW muss bei lokalem Zugriff auf die Messwerte über HAN an der Schnittstelle 
        /// IF_Adapter_TRuDI mind ein Datenelement vom Typ certId beinhalten, das auf ein HAN Zertifikat verweist.
        /// Sofern eine Signaturprüfung durchgeführt werden soll, müssen sowohl das WAN SIG als auch das zugehörige 
        /// SubCA Zertifikat referenziert werden.
        /// </summary>
        public List<byte> CertIds { get; set; } = new List<byte>();

        /// <summary>
        /// Firmware-Version des SMGW.
        /// </summary>
        public string FirmwareVersion { get; set; }

        /// <summary>
        /// Versionen bzw. Checksummen von ggf. vorhandenen Firmware-Komponenten (Betriebsystem, Applikation, etc.).
        /// </summary>
        public List<FirmwareComponent> FirmwareComponents { get; set; } = new List<FirmwareComponent>();
    }
}
