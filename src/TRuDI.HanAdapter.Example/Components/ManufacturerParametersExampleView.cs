namespace TRuDI.HanAdapter.Example.Components
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;

    public class ManufacturerParametersExampleView : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync()
        {
            return Task.FromResult<IViewComponentResult>(this.View());
        }
    }
}
