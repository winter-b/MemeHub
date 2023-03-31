using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using WebApi.Interfaces;
using WebApi.Models;
using WebApi.Repositories;
using WebApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using WebApi.Installers;
using Microsoft.OpenApi.Models;

namespace WebApi
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
            services.AddControllersWithViews();
            services.AddOpenTelemetryTracing(
            (builder) => builder
                      .SetResourceBuilder(
                        ResourceBuilder.CreateDefault().AddService("NetCoreWebApiTemplate"))
                       .AddAspNetCoreInstrumentation()
                       .AddConsoleExporter());


            services.AddControllers();

            services.AddSwaggerGen(c=>
             c.SwaggerDoc("v1", new OpenApiInfo { Title = "MemeHub API", Version = "v1", Description = "Reddit for blind programmers" }));


            var mongoConnectionString = Configuration.GetConnectionString("Mongo");
            services.AddSingleton<IAuthorizationService>(
                new AuthorizationService(
                    new AuthorizationRepository(
                        new MongoClient(mongoConnectionString)),
                    new EmailService(new EmailCreds() { 
                        Name = Configuration.GetValue<string>("EmailName"),
                        Password = Configuration.GetValue<string>("Password"),
                        Email = Configuration.GetValue<string>("FromEmail")}),
                    new HackingService(
                        new HackingRepository(
                            new MongoClient(mongoConnectionString))),
                    Configuration.GetValue<string>("salt"),
                    Configuration.GetValue<string>("key"),
                    Configuration.GetValue<string>("veriKey")));
            services.AddSingleton<IMemeService>(
                new MemeService(
                    new MemeRepository(
                        new MongoClient(
                            mongoConnectionString)),
                    new HackingService(
                        new HackingRepository(
                            new MongoClient(mongoConnectionString))),
                    Configuration.GetValue<string>("apiKey"),
                    Configuration.GetValue<string>("key")
                    ));
            services.AddSingleton<IHackingService>(
                new HackingService(
                    new HackingRepository(
                        new MongoClient(mongoConnectionString))));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();


            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Template API V1");
                c.RoutePrefix = "";
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
