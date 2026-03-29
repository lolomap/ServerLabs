using Serilog;

namespace Lab3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
                .WriteTo.Console()
                .WriteTo.File(
                    "logs/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{StatusCode}] {RequestPath} from {ClientIP} {NewLine}"
                )
                .CreateLogger();
            builder.Host.UseSerilog();

            // Add services to the container.
            builder.Services.AddRazorPages(options =>
            {
                options.Conventions.AddPageRoute("/Index", "index.html");
                options.Conventions.AddPageRoute("/Error404", "404.html");
                options.Conventions.AddPageRoute("/About", "about.html");
                options.Conventions.AddPageRoute("/Contact", "contact.html");
                options.Conventions.AddPageRoute("/Privacy", "privacy.html");
                options.Conventions.AddPageRoute("/ToS", "tos.html");
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSerilogRequestLogging(options =>
            {
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress?.ToString());
                };
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.UseStatusCodePagesWithReExecute("/Error404");

            app.Run();
        }
    }
}
