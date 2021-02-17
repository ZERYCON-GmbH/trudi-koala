namespace TRuDI.TafAdapter.Interface.Taf2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using TRuDI.HanAdapter.Interface;
    using TRuDI.Models;

    public static class RegisterExtensions
    {
        public static IList<Register> GetAccountingRegistersWithTotal(this IEnumerable<Register> registers)
        {
            var sortedRegisters = registers.ToList();
            sortedRegisters.Sort((a, b) => string.Compare(a.ObisCode.ToHexString(), b.ObisCode.ToHexString(), StringComparison.InvariantCulture));

            var totalImport = new Register { Amount = null, ObisCode = new ObisId("1-0:1.8.0*255"), TariffId = 0 };
            foreach (var reg in sortedRegisters)
            {
                if (reg.Amount.HasValue && reg.ObisCode.Medium == ObisMedium.Electricity && reg.ObisCode.C == 1
                    && reg.ObisCode.D == 8)
                {
                    if (totalImport.Amount == null)
                    {
                        totalImport.Amount = reg.Amount.Value;
                        totalImport.SourceType = reg.SourceType;
                    }
                    else
                    {
                        totalImport.Amount += reg.Amount.Value;
                    }
                }
            }

            if (totalImport.Amount != null)
            {
                sortedRegisters.Insert(sortedRegisters.FindLastIndex(r => r.ObisCode.Medium == ObisMedium.Electricity && r.ObisCode.C == 1 && r.ObisCode.D == 8) + 1, totalImport);
            }

            var totalExport = new Register { Amount = null, ObisCode = new ObisId("1-0:2.8.0*255"), TariffId = 0 };
            foreach (var reg in sortedRegisters)
            {
                if (reg.Amount.HasValue && reg.ObisCode.Medium == ObisMedium.Electricity && reg.ObisCode.C == 2
                    && reg.ObisCode.D == 8)
                {
                    if (totalExport.Amount == null)
                    {
                        totalExport.Amount = reg.Amount.Value;
                        totalExport.SourceType = reg.SourceType;
                    }
                    else
                    {
                        totalExport.Amount += reg.Amount.Value;
                    }
                }
            }

            if (totalExport.Amount != null)
            {
                sortedRegisters.Insert(sortedRegisters.FindLastIndex(r => r.ObisCode.Medium == ObisMedium.Electricity && r.ObisCode.C == 2 && r.ObisCode.D == 8) + 1, totalExport);
            }

            return sortedRegisters;
        }
    }
}
