namespace TRuDI.Backend
{
    using System;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Razor;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.FileProviders;

    using Serilog;
    using Serilog.Events;

    using TRuDI.Backend.Application;
    using TRuDI.Backend.MessageHandlers;
    using TRuDI.Backend.Utils;
    using TRuDI.HanAdapter.Repository;
    using TRuDI.TafAdapter.Interface;
    using TRuDI.TafAdapter.Repository;

    using WebSocketManager;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore()
                .AddViews()
                .AddRazorViewEngine()
                .AddJsonFormatters();

            services.AddWebSocketManager();
            services.AddSingleton<ApplicationState>();

            services.AddTransient<ITafData, ITafData>((ctx) =>
                {
                    var state = ctx.GetService<ApplicationState>();
                    return state.CurrentSupplierFile?.TafData?.Data;
                });

            services.Configure<RazorViewEngineOptions>(options =>
            {
                foreach (var hanAdapterInfo in HanAdapterRepository.AvailableAdapters)
                {
                    options.FileProviders.Add(new EmbeddedFileProvider(hanAdapterInfo.Assembly, hanAdapterInfo.BaseNamespace));
                }

                foreach (var tafAdapterInfo in TafAdapterRepository.AvailableAdapters)
                {
                    options.FileProviders.Add(new EmbeddedFileProvider(tafAdapterInfo.Assembly, tafAdapterInfo.BaseNamespace));
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            app.UseExceptionHandler("/Error/InternalError");

            if (Log.IsEnabled(LogEventLevel.Debug))
            {
                app.UseRequestLogging();
            }

            app.UseStaticFiles();
            app.UseWebSockets();

            app.MapWebSocketManager("/notifications", serviceProvider.GetService<NotificationsMessageHandler>());

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=OperatingModeSelection}/{action=Index}/{id?}");

                routes.MapRoute("resources", "{*path}", new { controller = "Ressources", action = "Get", path = string.Empty });
            });
        }
    }
}