namespace Shift_Tech.Models
{
    public static class HostBuilder
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
   Host.CreateDefaultBuilder(args)
       .ConfigureWebHostDefaults(webBuilder =>
       {
           webBuilder.UseStartup<Startup>();
       });
    }
}
