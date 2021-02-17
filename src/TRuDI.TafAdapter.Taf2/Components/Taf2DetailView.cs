namespace TRuDI.TafAdapter.Taf2.Components
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using TRuDI.TafAdapter.Interface;
    using TRuDI.TafAdapter.Interface.Taf2;

    public class Taf2DetailView : ViewComponent
    {
        private readonly ITaf2Data data;

        public Taf2DetailView(ITafData data)
        {
            this.data = data as ITaf2Data;
        }

        public Task<IViewComponentResult> InvokeAsync()
        {
            return Task.FromResult<IViewComponentResult>(this.View(this.data));
        }
    }
}
