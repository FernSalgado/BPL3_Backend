using BPL3_Backend.Models;
using BPL3_Backend.Repositories;
using BPL3_Backend.Services;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPL3_Backend
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
            services.Configure<DatabaseSettings>(Configuration.GetSection(nameof(DatabaseSettings)));
            services.AddSingleton<IDatabaseSettings>(x => x.GetRequiredService<IOptions<DatabaseSettings>>().Value);

            services.AddSingleton<ICsvToJSONService, CsvToJSONService>();
            services.AddSingleton<LadderService>();
            services.AddSingleton<WebScrappingService>();
            services.AddSingleton<TeamService>();
            services.AddSingleton<MemberService>();
            services.AddSingleton<TheFormedRepository>();
            services.AddSingleton<TheTwistedRepository>();
            services.AddSingleton<TheFearedRepository>();
            services.AddSingleton<TheHiddenRepository>();
            services.AddSingleton<TheFormedItemRepository>();
            services.AddSingleton<TheTwistedItemRepository>();
            services.AddSingleton<TheFearedItemRepository>();
            services.AddSingleton<TheHiddenItemRepository>();

            services.AddControllers();

            services.AddSwaggerGen();

            services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseMemoryStorage());

            services.AddHangfireServer();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IBackgroundJobClient backgroundJobs, IRecurringJobManager recurringJobManager, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseRouting();

            app.UseAuthorization();
            app.UseHangfireDashboard();
            recurringJobManager.AddOrUpdate(
                "Create base files",
                () => serviceProvider.GetService<ICsvToJSONService>().ReadTeamFile(),
                Cron.Monthly()
            );

            recurringJobManager.AddOrUpdate(
                "Get Items",
                () => serviceProvider.GetService<WebScrappingService>().ScrapperItems(),
                Cron.Hourly
                );

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
