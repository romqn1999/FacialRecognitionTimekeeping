using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FacialRecognitionTimekeepingAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FacialRecognitionTimekeepingAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<Services.BlockingCollectionPipelineAwaitable<string, bool>>(
                sp => new Services.BlockingCollectionPipelineAwaitable<string, bool>((builder) =>
                    //inputFirst.AddStep(builder, input => FindMostCommon(input))
                    //    .AddStep(builder, input => input.Length)
                    //    .AddStep(builder, input => input % 2 == 1)
                    builder.AddStep<string, string>(input => FindMostCommon(input))
                        .AddStep<string, int>(input => input.Length)
                        .AddStep<int, bool>(input => input % 2 == 1)
                )
            );

            services.AddSingleton<Services.FaceRecognitionTimekeepingPipelines>();

            //services.AddDbContext<TimekeepingContext>(opt => opt.UseInMemoryDatabase("Timekeepings").EnableSensitiveDataLogging(), ServiceLifetime.Singleton);
            services.AddDbContext<TimekeepingContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("TimekeepingContext")).EnableSensitiveDataLogging(), ServiceLifetime.Singleton);

            services.AddControllers();
        }

        private static string FindMostCommon(string input)
        {
            Console.WriteLine(nameof(FindMostCommon));
            Console.WriteLine(input);
            return input.Split(' ')
                .GroupBy(word => word)
                .OrderBy(group => group.Count())
                .Last()
                .Key;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var timekeepingContext = serviceScope.ServiceProvider.GetRequiredService<TimekeepingContext>();
                timekeepingContext.Database.Migrate();
                timekeepingContext.Database.EnsureCreated();
            }

            FaceRecognitionTimekeepingPipelines.Logger = app.ApplicationServices.GetService<ILogger<FaceRecognitionTimekeepingPipelines>>();
            CognitiveServiceApiRequest.Logger = app.ApplicationServices.GetService<ILogger<CognitiveServiceApiRequest>>();
        }
    }
}
