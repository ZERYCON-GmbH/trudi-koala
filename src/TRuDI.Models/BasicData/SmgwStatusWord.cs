namespace TRuDI.Models.BasicData
{
    using System;

    [Flags]
    public enum SmgwStatusWord : uint
    {
        SmgwStatusWordIdentification = 0x05,
        Transparency_Bit = 0x2,
        Fatal_Error = 0x100,
        Systemtime_Invalid = 0x200,
        PTB_Warning = 0x1000,
        PTB_Temp_Error_signed_invalid = 0x2000,
        PTB_Temp_Error_is_invalid = 0x4000,
    }
}