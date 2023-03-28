using ClientGUI.Connectors;

namespace ClientGUI
{
    public class Program
    {
        private static string SENTIMENT_SOURCE = @"http://5400-project-sentiment_analysis-1:8000/analyze";
        private static string DATABASE_SOURCE = "Server=5400-project-db-1;Port=5432;Database=DataAnalysis;User Id=root;Password=CSCI5400;";

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews(); 
            builder.Services.AddSingleton<ISentiment>(new SentimentAnalyzer(SENTIMENT_SOURCE));  //Uses instance of our Python Sentiment Analyzer when injecting dependency into controller
            builder.Services.AddSingleton<IDataSource>(new PostgresDataSource(DATABASE_SOURCE)); //Uses instance of our postgres database when injecting dependency into controller

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