using Microsoft.Extensions.Hosting;
using Web.Framework;

namespace Web.Samples.OrderManagement.API
{
    public class Program : WebProgram<Startup>
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
    }
}
