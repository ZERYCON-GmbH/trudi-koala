namespace TRuDI.HanAdapter.Example.Components
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;

    public class GatewayImageExampleView : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync(string deviceId)
        {
            return Task.FromResult<IViewComponentResult>(this.View(new GatewayImageExampleViewModel { DeviceId = deviceId }));
        }
    }
}
