namespace TRuDI.Backend.Controllers
{
    using System;
    using System.Linq;

    using Microsoft.AspNetCore.Mvc;

    using TRuDI.Backend.Application;
    using TRuDI.HanAdapter.Interface;
    using TRuDI.Models;

    public class ContractsController : Controller
    {
        private readonly ApplicationState applicationState;

        public ContractsController(ApplicationState applicationState)
        {
            this.applicationState = applicationState;
        }

        public IActionResult Index()
        {
            this.applicationState.BreadCrumbTrail.Add("Verträge", "/Contracts", false);
            this.applicationState.SideBarMenu.Clear();

            this.ViewData["ErrorMessage"] = this.applicationState.LastErrorMessages.FirstOrDefault();
            return this.View();
        }
        
        public IActionResult StartReadout(
            int contractIndex,
            int billingPeriodIndex,
            DateTime startTime,
            DateTime endTime,
            string mode)
        {
            var contractContainer = this.applicationState.Contracts[contractIndex];
            var contract = mode == "BP" ? contractContainer.Contract : contractContainer.Taf6;

            if (contract == null)
            {
                return this.NotFound();
            }

            var billingPeriods = contract.BillingPeriods.OrderByDescending(bp => bp.Begin).ToArray();

            var ctx = new AdapterContext
            {
                Contract = contract,
                BillingPeriod = billingPeriods[billingPeriodIndex],
                Start = startTime,
                End = endTime,
            };

            if (!ctx.BillingPeriod.IsCompleted() || ctx.BillingPeriod.End?.ToUniversalTime() != ctx.End.ToUniversalTime())
            {
                ctx.End = ctx.End.NextDayStart();
            }

            if (ctx.BillingPeriod.Begin != ctx.Start)
            {
                ctx.Start = ctx.Start.DayStart();
            }

            ctx.WithLogdata = true;
            this.applicationState.LoadData(ctx);

            return this.Ok();
        }

        public IActionResult StartReadoutTaf7(
            int contractIndex,
            DateTime startTime,
            DateTime endTime)
        {
            var contractContainer = this.applicationState.Contracts[contractIndex];

            var ctx = new AdapterContext
                          {
                              Contract = contractContainer.Contract,
                              BillingPeriod = null,
                              Start = startTime.DayStart(),
                              End = endTime.NextDayStart(),
                          };

            ctx.WithLogdata = true;
            this.applicationState.LoadData(ctx);

            return this.Ok();
        }
    }
}
