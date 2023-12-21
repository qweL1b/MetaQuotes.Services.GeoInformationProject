
using System.Net;
using MetaQuotes.Services.GeoInformationService.Application;
using MetaQuotes.Services.GeoInformationService.Application.Services;
using MetaQuotes.Services.GeoInformationService.Infrastructure.Services;
using Microsoft.AspNetCore.Diagnostics;

namespace MetaQuotes.Services.GeoInformationService.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                  .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

            builder.Configuration.AddEnvironmentVariables();

            builder.Services.AddControllers();

            builder.Services.AddSingleton<GeoBaseService>(); //adding a service for searching by geolocation
            builder.Services.AddSingleton<ILoadDatabase>(provider => provider.GetRequiredService<GeoBaseService>());
            builder.Services.AddSingleton<IGeoSearching>(provider => provider.GetRequiredService<GeoBaseService>());

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(); //adding a Swagger

            var app = builder.Build();

            var geoBaseService = app.Services.GetRequiredService<ILoadDatabase>();
            var isDataLoaded = geoBaseService.LoadData("geobase.dat");  //Loading data from the database geobase.dat

            if (isDataLoaded)
                Console.WriteLine("Data loaded successfully!");
            else
                Console.WriteLine("Data loaded unsuccessfully!");

            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        Console.WriteLine($"An error has occurred: {contextFeature.Error}");

                        await context.Response.WriteAsync(new ErrorDetails
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = "An internal server error has occurred."
                        }.ToString());
                    }
                });
            });

            
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseStaticFiles(); 

            app.UseAuthorization();

            app.MapControllers();

            app.MapFallbackToFile("index.html");

            app.Run();
        }
    }
}