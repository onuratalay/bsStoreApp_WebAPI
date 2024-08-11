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
                    // true yaparak API projemizi i�erik pazarl���na a��k hale getiriyoruz.

                    config.ReturnHttpNotAcceptable = true;
                    // Kabul etmedi�imiz formatlar�n hata kodunu d�nebilmesi i�in true olarak de�i�tiriyoruz.
                })

                .AddCustomCsvFormatter()
                .AddXmlDataContractSerializerFormatters()
                // XML format�nda content verebilmesini sa�lar.
                .AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly)
                .AddNewtonsoftJson();

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
                // [ApiController] anonasyonu eklendi�i zaman, Controller baz� default �zellikler (BehaviorOptions) kazan�r. Bununla gelen �zelliklerden birisi de ModelStateInvalidFilter. Bu yap� ge�ersiz bir model state i ile kar��la��rsa 400 kodunu otomatik olarak �retiyor.

                // DataAnnotation ile validasyon yapt���m�zda,
                // invalid bir veri girilirse otomatik olu�an
                // 400 bad requesti suppressler (bast�r�r).
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
