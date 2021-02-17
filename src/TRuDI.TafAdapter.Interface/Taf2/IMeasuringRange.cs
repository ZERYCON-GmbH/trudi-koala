namespace TRuDI.TafAdapter.Interface.Taf2
{
    using System;

    public interface IMeasuringRange
    {
        DateTime Start { get; }
        DateTime End { get; }
        ushort TariffId { get; }
        long Amount { get; }
    }
}
