namespace TRuDI.Models.BasicData
{
    using System;

    [Flags]
    public enum BzStatusWord : uint
    {
        BzStatusWordIdentification = 0x04,
        Start_Up = 0x100,
        Magnetically_Influenced = 0x200,
        Manipulation_KD_PS = 0x400,
        Sum_Energiedirection_neg = 0x800,
        Energiedirection_L1_neg = 0x1000,
        Energiedirection_L2_neg = 0x2000,
        Energiedirection_L3_neg = 0x4000,
        PhaseSequenz_RotatingField_Not_L1_L2_L3 = 0x8000,
        BackStop_Active = 0x10000,
        Fatal_Error = 0x20000,
        Lead_Voltage_L1_existent = 0x40000,
        Lead_Voltage_L2_existent = 0x80000,
        Lead_Voltage_L3_existent = 0x100000
    }
}