using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.SelfHost;
using Microsoft.Owin.Hosting;

namespace YoutubeQueuer.Web.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            
            var baseAddress = "http://localhost:9000/";
            // Start OWIN host 
            //AreaRegistration.RegisterAllAreas();
            //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //RouteConfig.RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);

            var config = new HttpSelfHostConfiguration(baseAddress);
            config.Routes.MapHttpRoute("default", "{controller}");
            var startOptions = new StartOptions(baseAddress)
            {
                
            };

            
            //using (var svr = new HttpSelfHostServer(config))
            //{
            //    svr.OpenAsync().Wait();
            //    Console.WriteLine("Press Enter to quit.");
            //    Console.ReadLine();
            //}

            var app = WebApp.Start<Startup>(url: baseAddress);
            
            while (true)
            {
                var input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input) && input.ToLower()[0] == 'q')
                {
                    break;
                }
                // Create HttpCient and make a request to api/values 
                var client = new HttpClient();

                var response = client.GetAsync(baseAddress).Result;

                Console.WriteLine(response);
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                Console.ReadLine();
            }
            app.Dispose();
        }
    }
}
