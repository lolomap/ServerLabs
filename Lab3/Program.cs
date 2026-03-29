namespace Lab3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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
