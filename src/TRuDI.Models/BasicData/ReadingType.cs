namespace TRuDI.Models.BasicData
{
    /// <summary>
    /// Die Klasse ReadingType spezifiziert die Inhalte einer Messwertliste.
    /// 
    /// Jede Messwertliste muss eine Instanz der Klasse ReadingType beinhalten.
    /// 
    /// Die Klasse ReadyType verweist auf keine weiteren Klassen.
    /// </summary>
    public class ReadingType
    {
        /// <summary>
        /// Verweis auf die MeterReading Instanz, die die Instanz der ReadingType 
        /// Klasse beinhaltet.
        /// </summary>
        public MeterReading MeterReading
        {
            get; set;
        }

        /// <summary>
        /// Das Datenelement powerOfTenMultiplier repräsentiert den Einheitenvorsatz der Maßeinheit der in der Messwertliste übermittelten Werte.
        /// Gültige Werte nach ESPI REQ.21 sind:
        /// 
        ///         0 = None
        ///         1 = deca= x10
        ///         2 = hecto= x100
        ///        –3 = mili= x10–3
        ///         3 = kilo= x1000
        ///         6 = Mega= x106
        ///        –6 = micro= x10–3
        ///         9 = Giga= x109
        ///         
        /// Das Datenelement powerofTenMultiplier muss mit einem entsprechenden Wert gefüllt werden.
        /// </summary>
        public PowerOfTenMultiplier? PowerOfTenMultiplier
        {
            get; set;
        }

        /// <summary>
        /// Das Datenelement scaler repräsentiert den Skalierungsfaktor der ganzahligen Messwerte 
        /// (IntervalReading–value) in der Messwertliste. Durch diesen kann eine Kommaverschiebung 
        /// für den Messwert dargestellt werden.
        /// </summary>
        public short Scaler
        {
            get; set;
        }

        /// <summary>
        /// Das Datenelement uom codiert die Maßeinheit, welche für alle Messwerte der Messwertliste gilt. 
        /// Gültige Werte nach ESPI REQ.21 sind:
        /// 
        ///     0 = Not Applicable
        ///     5 = A(Current)
        ///     29 = Voltage
        ///     31 = J(Energy joule)
        ///     33 = Hz(Frequency)
        ///     38 = Real power(Watts)
        ///     42 = m^3(Cubic Meter)
        ///     61 = VA(Apparent power)
        ///     63 = VAr(Reactive power)
        ///     65 = CosPhi(Power factor)
        ///     67 = V^2(Volts squared)
        ///     69 = A^2(Amp squared)
        ///     71 = VAh(Apparent energy)
        ///     72 = Real energy(Watt-hours)
        ///     73 = VArh(Reactive energy)
        ///     106 = Ah(Ampere-hours / Available Charge)
        ///     119 = ft^3(Cubic Feet)
        ///     122 = ft^3/h(Cubic Feet per Hour)
        ///     125 = m^3/h(Cubic Meter per Hour)
        ///     128 = US gl(US Gallons)
        ///     129 = US gl/h(US Gallons per Hour)
        ///     
        ///Das Datenelement uom muss mit einem entsprechenden Wert gefüllt werden.
        /// </summary>
        public Uom? Uom
        {
            get; set;
        }

        /// <summary>
        /// Das Datenelement obisCode codiert die Messwerte der Messwertliste wie im Object Identification System (OBIS) 
        /// nach DIN EN 62056-6-1 und für die Nutzung in den EDIFACT-Nachrichtentypen des deutschen Energiemarktes beschrieben.
        /// Das Datenelement obisCode muss mit einem entsprechenden Wert(OBIS-Kennzahl) gefüllt werden.
        /// Dastellung als HexCode ohne Trennzeichen im einem String-Datenformat.
        /// </summary>
        public string ObisCode
        {
            get; set;
        }

        /// <summary>
        /// Das Datenelement qualifiedLogicalName ist ein eindeutiger Bezeichner einer Messreihe, 
        /// die von einem Smart Meter Gateway übertragen werden kann. Er setzt sich nach der COSEM-Definition 
        /// zusammen aus <OBIS>.<SMGW-ID>.sm und wird für die Bildung der inneren Signatur genutzt.
        /// 
        ///Jede Instanz der Klasse readingType muss ein Datenelement qualifiedLogicalName enthalten.
        /// </summary>
        public string QualifiedLogicalName
        {
            get; set;
        }

        /// <summary>
        /// Die verwendete Messperiode in Sekunden. 
        /// </summary>
        public uint MeasurementPeriod { get; set; }
    }
}
