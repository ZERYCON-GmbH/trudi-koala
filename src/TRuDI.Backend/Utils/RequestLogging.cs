namespace TRuDI.Backend.Utils
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    using Serilog;

    /// <summary>
    /// Used to log requests to the ASP.Net controllers.
    /// </summary>
    public class RequestLogging
    {
        /// <summary>
        /// The next request delegate.
        /// </summary>
        private readonly RequestDelegate next;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestLogging"/> class.
        /// </summary>
        /// <param name="next">The next request delegate.</param>
        public RequestLogging(RequestDelegate next)
        {
            this.next = next;
        }

        /// <summary>
        /// This is called for each request.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns>The result of the request.</returns>
        public async Task Invoke(HttpContext context)
        {
            var sw = Stopwatch.StartNew();

            Log.Debug(
                "Backend request: {0} {1}{2} {3}",
                context.Request.Scheme,
                context.Request.Host,
                context.Request.Path,
                DateTime.Now.ToString("HH:mm:ss.fff"));

            await this.next(context);

            Log.Debug(
                "Backend response: {0}, {1} ms",
                context.Response.StatusCode,
                sw.ElapsedMilliseconds);
        }
    }
}
