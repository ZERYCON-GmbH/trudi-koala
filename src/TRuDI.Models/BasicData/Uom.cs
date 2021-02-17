namespace TRuDI.Models.BasicData
{
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
    public enum Uom : ushort
    {
        Not_Applicable = 0,
        Ampere = 5,
        AngleDegrees = 9,
        Volltage = 29,
        Joule = 31,
        Frequency = 33,
        Real_power = 38,
        Cubic_meter = 42,
        Apparent_power = 61,
        Reactive_power = 63,
        Power_factor = 65,
        Volts_squared = 67,
        Ampere_squared = 69,
        Apparent_energy = 71,
        Real_energy = 72,
        Reactive_energie = 73,
        Ampere_hours = 106,
        Cubic_feet = 119,
        Cubic_feet_per_hour = 122,
        Cubic_meter_per_hour = 125,
        US_Gallons = 128,
        US_Gallons_per_hour = 129,
    }
}
