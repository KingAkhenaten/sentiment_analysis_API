using ClientGUI.Connectors;

namespace ClientGUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews(); 
            builder.Services.AddSingleton<ISentiment>(new SentimentAnalyzer());   //Uses instance of our Python Sentiment Analyzer when injecting dependency into controller
            builder.Services.AddSingleton<IDataSource>(new PostgresDataSource()); //Uses instance of our postgres database when injecting dependency into controller

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}