using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoreMiddleware
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Information);
            var logger = loggerFactory.CreateLogger("logger factory demo");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<SampleMiddleware>();//because UseStaticFiles is terminating if we want adding custom headers we should put it before UseStaticFiles . but still it cannot inject content to fixed static html

            app.UseStaticFiles();

            app.Use(async (context, next) =>
            {
                Stopwatch sw = Stopwatch.StartNew();
                
                logger.LogInformation($"--- from middle ware start");
                await next();
                logger.LogInformation($"--- from middle ware start. elapsed : {sw.ElapsedMilliseconds}");
            }
            );

            //app.Map("/Stuff", a => a.Run(MapStuff));
            app.Map("/Stuff", a=> a.Run(async (context) =>            {
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync("Hello World!");
            })
            );

            //app.UseAuthentication();
            app.MapWhen(c=>c.Request.Headers["User-Agent"].Contains("chrome"),routeChrome);
            app.Run(  async (context) =>
            {
                context.Response.ContentType = "text/html";

                await context.Response.WriteAsync("Hello World!");
            });
        }

        private void routeChrome(IApplicationBuilder app)
        {
            app.Run(async c => await c.Response.WriteAsync("Chrome response mapped"));
        }

        private async Task MapStuff(HttpContext context)
        {

            context.Response.ContentType = "text/html";
            await context.Response.WriteAsync("Here is your stuff content");
        }
    }
}
