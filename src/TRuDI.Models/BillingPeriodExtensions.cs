namespace TRuDI.Models
{
    using System;
    using TRuDI.HanAdapter.Interface;

    public static class BillingPeriodExtensions
    {
        public static bool IsCompleted(this BillingPeriod billingPeriod)
        {
            if (billingPeriod == null)
            {
                return false;
            }

            return billingPeriod.End != null && billingPeriod.End <= DateTime.Now;
        }
    }
}
