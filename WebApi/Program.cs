using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog;
using Presentation.ActionFilters;
using Repositories.EFCore;
using Services.Contracts;
using WebApi.Extensions;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            LogManager.Setup().LoadConfigurationFromFile(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

            builder.Services.AddControllers(config =>
                {
                    config.RespectBrowserAcceptHeader = true;
                    // Content Negotiation default olarak false gelir. (Accept : */*)
                    // true yaparak API projemizi içerik pazarlýðýna açýk hale getiriyoruz.

                    config.ReturnHttpNotAcceptable = true;
                    // Kabul etmediðimiz formatlarýn hata kodunu dönebilmesi için true olarak deðiþtiriyoruz.
                })

                .AddCustomCsvFormatter()
                .AddXmlDataContractSerializerFormatters()
                // XML formatýnda content verebilmesini saðlar.
                .AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly)
                .AddNewtonsoftJson();

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
                // [ApiController] anonasyonu eklendiði zaman, Controller bazý default özellikler (BehaviorOptions) kazanýr. Bununla gelen özelliklerden birisi de ModelStateInvalidFilter. Bu yapý geçersiz bir model state i ile karþýlaþýrsa 400 kodunu otomatik olarak üretiyor.

                // DataAnnotation ile validasyon yaptýðýmýzda,
                // invalid bir veri girilirse otomatik oluþan
                // 400 bad requesti suppressler (bastýrýr).
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.ConfigureSqlContext(builder.Configuration); // dbcontext
            builder.Services.ConfigureRepositoryManager();
            builder.Services.ConfigureServiceManager();
            builder.Services.ConfigureLoggerService();
            builder.Services.AddAutoMapper(typeof(Program));
            builder.Services.ConfigureActionFilters();

            var app = builder.Build();

            var logger = app.Services.GetRequiredService<ILoggerService>();
            app.ConfigureExceptionHandler(logger);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            if (app.Environment.IsProduction())
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
