namespace TRuDI.HanAdapter.Interface
{
    /// <summary>
    /// Medium definitions based on the OBIS system.
    /// </summary>
    public enum ObisMedium : byte
    {
        /// <summary>
        /// Abstract.
        /// </summary>
        Abstract = 0x00,

        /// <summary>
        /// Electricity.
        /// </summary>
        Electricity = 0x01,

        /// <summary>
        /// Heat cost allocator.
        /// </summary>
        HeatCostAllocator = 0x04,

        /// <summary>
        /// Cooling.
        /// </summary>
        Cooling = 0x05,

        /// <summary>
        /// Heat.
        /// </summary>
        Heat = 0x06,

        /// <summary>
        /// Gas.
        /// </summary>
        Gas = 0x07,

        /// <summary>
        /// Cold water.
        /// </summary>
        WaterCold = 0x08,

        /// <summary>
        /// Hot water.
        /// </summary>
        WaterHot = 0x09,

        /// <summary>
        /// Communication related.
        /// </summary>
        Communication = 0x0E,
    }
}
