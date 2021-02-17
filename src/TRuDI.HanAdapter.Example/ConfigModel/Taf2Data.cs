namespace TRuDI.HanAdapter.Example.ConfigModel
{
    using System;
    using System.Collections.Generic;

    using TRuDI.Models;

    /// <summary>
    /// Storage for Taf2 data. 
    /// It is used to create the derived registers.
    /// </summary>
    class Taf2Data
    {
        public Taf2Data(ObisId obisID)
        {
            this.ObisID = obisID;
            this.Data = new List<(DateTime timestamp, int tariff, int value)>();
        }

        public ObisId ObisID { get; set; }

        public List<(DateTime timestamp, int tariff, int value)> Data { get; set; }
    }
}
