using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace CoreMiddleware
{
    internal class SampleMiddleware
    {
        public SampleMiddleware(RequestDelegate next,IHostingEnvironment hostingEnvironment)
        {
            this.Next = next;
            this.EnvironmentName = hostingEnvironment.EnvironmentName;
            this.ApplicationName = hostingEnvironment.ApplicationName;
        }

        public RequestDelegate Next { get; }
        public string EnvironmentName { get; }
        public string ApplicationName { get; }

        public async Task Invoke(HttpContext context)
        {            
            context.Response.Headers.Add("X-IHostingEnvironment", EnvironmentName);

            await Next(context);

            if (context.Response.ContentType != null && context.Response.ContentType.Contains("html"))
                await context.Response.WriteAsync($"<p style=\"font-size:88px;background-color:red\"> test : {DateTime.Now}</p>");
            

        }

    }
}