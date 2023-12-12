
using System.Net;
using MetaQuotes.Services.GeoInformationService.Application;
using MetaQuotes.Services.GeoInformationService.Application.Services;
using MetaQuotes.Services.GeoInformationService.Infrastructure.Services;
using Microsoft.AspNetCore.Diagnostics;

namespace MetaQuotes.Services.GeoInformationService.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddSingleton<GeoBaseService>();
            builder.Services.AddSingleton<ILoadDatabase>(provider => provider.GetRequiredService<GeoBaseService>());
            builder.Services.AddSingleton<IGeoSearching>(provider => provider.GetRequiredService<GeoBaseService>());

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            var geoBaseService = app.Services.GetRequiredService<ILoadDatabase>();
            geoBaseService.LoadDataAsync("geobase.dat");

            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        Console.WriteLine($"Произошла ошибка: {contextFeature.Error}");

                        await context.Response.WriteAsync(new ErrorDetails
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = "An internal server error has occurred."
                        }.ToString());
                    }
                });
            });

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles(); 

            app.UseAuthorization();

            app.MapControllers();

            app.MapFallbackToFile("index.html");

            app.Run();
        }
    }
}