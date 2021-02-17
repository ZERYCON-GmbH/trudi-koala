namespace TRuDI.HanAdapter.Test.Commands.ContractList
{
    using System;
    using System.Linq;

    using TRuDI.HanAdapter.Interface;

    public class Contract
    {
        private ContractInfo ci;

        public Contract()
        {
        }

        public Contract(ContractInfo ci)
        {
            this.ci = ci;
        }
        
        public TafId TafId
        {
            get => this.ci.TafId;
            set => this.ci.TafId = value;
        }

        public string TafName
        {
            get => this.ci.TafName;
            set => this.ci.TafName = value;
        }

        public string Description
        {
            get => this.ci.Description;
            set => this.ci.Description = value;
        }

        public string[] Meters
        {
            get => this.ci.Meters.ToArray();
            set => throw new NotImplementedException();
        }

        public string MeteringPointId
        {
            get => this.ci.MeteringPointId;
            set => this.ci.MeteringPointId = value;
        }

        public string SupplierId
        {
            get => this.ci.SupplierId;
            set => this.ci.SupplierId = value;
        }

        public string ConsumerId
        {
            get => this.ci.ConsumerId;
            set => this.ci.ConsumerId = value;
        }

        public DateTime Begin
        {
            get => this.ci.Begin;
            set => this.ci.Begin = value;
        }

        public DateTime? End
        {
            get => this.ci.End;
            set => this.ci.End = value;
        }

        public BillingPeriod[] BillingPeriods
        {
            get => this.ci.BillingPeriods?.ToArray();
            set => throw new NotImplementedException();
        }
    }
}
