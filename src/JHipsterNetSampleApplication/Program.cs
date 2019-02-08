using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace JHipsterNetSampleApplication {
    public class Program {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(params string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
        }
    }
}
