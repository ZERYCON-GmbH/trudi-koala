namespace TRuDI.Models.BasicData
{
    /// <summary>
    /// Die Klasse Certificate repärsentiert das Zertifikat, welches für die Inhalstdatensignierung (WAN SIG),
    /// für die TSL-Verschlüsselung am HAN oder für die Signierung von Zertifikaten durch eine SubCA genutzt 
    /// wird. Wird von einer anderen Rolle als dem SMGW ein Messwert signiert (z.B. bei manueller Änderung 
    /// eines Messwerts), so ist das entsprechende Zertifikat hier zusätzlich einzufügen. 
    ///
    /// Für eine eichrechtlich-konforme Überprüfung der Daten muss die Nachricht mindestens eine Instanz der 
    /// Klasse Certificate beinhalten.
    ///
    /// Die Klasse Certificate verweist auf keine weiteren Klassen.
    /// </summary>
    public class Certificate
    {
        /// <summary>
        /// Das Datenelement certId identifiziert ein Zertifikat eindeutig. Das Zertifikat des SMGW hat per 
        /// Default die ID mit der Nummer 1. Jede Instanz der Klasse Certificate muss ein Datenelement vom Typ
        /// CertId enthalten.
        /// </summary>
        public byte? CertId
        {
            get; set;
        }

        /// <summary>
        /// Das Datenelement certType klassifiziert ein Zertifikat eindeutig als:
        ///
        ///     1 = Signatur-Zertifikat
        ///     2 = SubCA-Zertifikat
        ///     3 = SMGW-HAN-Zertifikat
        ///
        /// Jede Instanz der Klasse Certificate muss ein Datenelement vom Typ certType enthalten.
        /// </summary>
        public CertType? CertType
        {
            get; set;
        }

        /// <summary>
        /// Das Datenelement parentCertId enthält die ID des Zertifikats der nächts höheren SubCA 
        /// in der Zertifikatskette.
        ///
        /// Jede Instanz der Klasse Certificate kann ein Datenelement vom Typ parentCertId enthalten.
        /// </summary>
        public byte? ParentCertId
        {
            get; set;
        }

        /// <summary>
        /// Das Datenelement certContent enthält das eingentliche Zertifikat. 
        ///
        /// Jede Instanz der Klasse Certificate muss ein Datenelement vom Typ
        /// certContent enthalten.
        ///
        /// Die Schemadatei definiert als Datentyp hexBinary. Daher das byte-Feld.
        /// https://msdn.microsoft.com/de-de/library/system.xml.serialization.xmlelementattribute.datatype(v=vs.110).aspx
        /// </summary>
        public byte[] CertContent
        {
            get; set;
        }
    }
}
