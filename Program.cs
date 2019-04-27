using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
//using test2.Services.BackgroundServices;

namespace test2
{
    public class Program
    {
        public static void Main(string[] args)
        {
           
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("https://10.10.0.189:5566") // have to change IP before use
               // .UseContentRoot(Directory.GetCurrentDirectory()) 
                //.UseIISIntegration()
                //.UseShutdownTimeout(TimeSpan.FromSeconds(10))
                .UseStartup<Startup>();
    }
}
