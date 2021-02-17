namespace TRuDI.Backend.Utils
{
    using Microsoft.AspNetCore.Builder;

    /// <summary>
    /// This extension class is used to configure the request logging.
    /// </summary>
    public static class RequestLoggingExtensions
    {
        /// <summary>
        /// Enables the request logging middle ware.
        /// </summary>
        /// <param name="builder">The application builder.</param>
        /// <returns>The application builder.</returns>
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLogging>();
        }
    }
}
