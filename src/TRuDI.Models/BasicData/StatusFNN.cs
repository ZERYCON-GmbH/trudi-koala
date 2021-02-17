namespace TRuDI.Models.BasicData
{
    using System;

    /// <summary>
    /// Kombiniertes Statuswort zu einem Messwert (nach FNN LH SMGw).
    /// </summary>
    /// <remarks>
    /// Soweit ein Messwert ein Statuswort mitführt, muss dieses als kombiniertes Statuswort aus dem Statuswort des Messwert-Gebers (i.d.R. ein Sensor bzw. Zähler)
    /// und dem Statuswort des SMGW gebildet werden. Die Reihenfolge der Bits ist durch das FNN etwas unkonventionelle auf "Least Significant Bit First" festgelegt worden.
    /// D.h. das Statuswort wird wie folgt erwaret: "A000000020000000"
    /// </remarks>
    public class StatusFNN
    {
        public StatusFNN(string status)
        {
            this.Status = status.PadLeft(16, '0');
            var smgwStat = Reverse(Convert.ToUInt32(this.Status.Substring(0, 8), 16));
            var bzStat = Reverse(Convert.ToUInt32(this.Status.Substring(8), 16));
            this.SmgwStatusWord = (SmgwStatusWord)smgwStat;
            this.BzStatusWord = (BzStatusWord)bzStat;
        }

        public StatusFNN(ulong status)
        {
            var smgwStat = (uint)(status >> 32);
            var bzStat = (uint)(status & 0xFFFFFFFF);
            this.SmgwStatusWord = (SmgwStatusWord)smgwStat;
            this.BzStatusWord = (BzStatusWord)bzStat;

            this.Status = Reverse(smgwStat).ToString("X8") + Reverse(bzStat).ToString("X8");
        }

        public StatusFNN(SmgwStatusWord smgwStat, BzStatusWord bzStat)
        {
            this.SmgwStatusWord = smgwStat;
            this.BzStatusWord = bzStat;

            this.Status = Reverse((uint)smgwStat).ToString("X8") + Reverse((uint)bzStat).ToString("X8");
        }

        public string Status
        {
            get; set;
        }

        public SmgwStatusWord SmgwStatusWord
        {
            get; set;
        }

        public BzStatusWord BzStatusWord
        {
            get; set;
        }

        public StatusPTB MapToStatusPtb()
        {
            if (this.BzStatusWord.HasFlag(BzStatusWord.Fatal_Error) ||
                this.SmgwStatusWord.HasFlag(SmgwStatusWord.Fatal_Error))
            {
                return StatusPTB.FatalError;
            }

            if (this.SmgwStatusWord.HasFlag(SmgwStatusWord.Systemtime_Invalid) ||
                this.SmgwStatusWord.HasFlag(SmgwStatusWord.PTB_Temp_Error_is_invalid) ||
                this.BzStatusWord.HasFlag(BzStatusWord.Manipulation_KD_PS) ||
                this.BzStatusWord.HasFlag(BzStatusWord.Magnetically_Influenced))
            {
                return StatusPTB.CriticalTemporaryError;
            }

            if (this.SmgwStatusWord.HasFlag(SmgwStatusWord.PTB_Temp_Error_signed_invalid))
            {
                return StatusPTB.TemporaryError;
            }

            if (this.SmgwStatusWord.HasFlag(SmgwStatusWord.PTB_Warning))
            {
                return StatusPTB.Warning;
            }

            return StatusPTB.NoError;
        }

        private static uint Reverse(uint x)
        {
            uint y = 0;
            for (int i = 0; i < 32; ++i)
            {
                y <<= 1;
                y |= (x & 1);
                x >>= 1;
            }

            return y;
        }

        public bool Validate()
        {
            return this.BzStatusWord.HasFlag(BzStatusWord.BzStatusWordIdentification)
                   && this.SmgwStatusWord.HasFlag(SmgwStatusWord.SmgwStatusWordIdentification);
        }
    }
}
