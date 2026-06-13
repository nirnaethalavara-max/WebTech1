using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Kakhanouskaya.UI.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class FileLoggerMiddleware
    {
        private readonly RequestDelegate _next;

        public FileLoggerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Выклікаем наступны middleware (каб сфармаваўся адказ)
            await _next(context);

            // Атрымліваем код адказу
            int statusCode = context.Response.StatusCode;
            int statusFamily = statusCode / 100;

            // Калі код не 2xx (200-299) - лагіруем
            if (statusFamily != 2)
            {
                string path = context.Request.Path;
                Log.Information("---> request {Path} returns {StatusCode}", path, statusCode);
            }
        }
    }

    // Extension method для лёгкай рэгістрацыі
    public static class FileLoggerMiddlewareExtensions
    {
        public static IApplicationBuilder UseFileLogger(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<FileLoggerMiddleware>();
        }
    }
}
