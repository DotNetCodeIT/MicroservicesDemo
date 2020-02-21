using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Suppliers.Data;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Suppliers
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
            string connection = Environment.GetEnvironmentVariable("SQL_CONNECTIONSTRING") ?? Configuration["ConnectionStrings:SuppliersContext"];
            services.AddControllers();
            services.AddHealthChecks()
                .AddSqlServer(connection);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Suppliers API", Version = "v1" });
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyOrigin().AllowAnyMethod();
                    });
            });

            services.AddDbContext<SuppliersContext>(options =>
                options.UseSqlServer(connection));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Add a custom response header 
            app.Use(async (context, nextMiddleware) =>
            {
                context.Response.OnStarting(() =>
                {
                    context.Response.Headers.Add("MY_NODE_NAME", Environment.GetEnvironmentVariable("MY_NODE_NAME") ?? "");
                    context.Response.Headers.Add("MY_POD_NAME", Environment.GetEnvironmentVariable("MY_POD_NAME") ?? "");
                    context.Response.Headers.Add("MY_POD_IP", Environment.GetEnvironmentVariable("MY_POD_IP") ?? "");
                    context.Response.Headers.Add("MY_POD_SERVICE_ACCOUNT", Environment.GetEnvironmentVariable("MY_POD_SERVICE_ACCOUNT") ?? "");
                    context.Response.Headers.Add("MY_CPU_REQUEST", Environment.GetEnvironmentVariable("MY_CPU_REQUEST") ?? "");
                    context.Response.Headers.Add("MY_CPU_LIMIT", Environment.GetEnvironmentVariable("MY_CPU_LIMIT") ?? "");
                    context.Response.Headers.Add("MY_MEM_REQUEST", Environment.GetEnvironmentVariable("MY_MEM_REQUEST") ?? "");
                    context.Response.Headers.Add("MY_MEM_LIMIT", Environment.GetEnvironmentVariable("MY_MEM_LIMIT") ?? "");

                    return Task.FromResult(0);
                });
                await nextMiddleware();
            });


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDeveloperExceptionPage();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Suppliers V1");
                });
            }

            app.UseHttpsRedirection();

            app.UseCors();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    if (env.IsDevelopment())
                    {
                        foreach (string item in Environment.GetEnvironmentVariables().Keys)
                        {
                            await context.Response.WriteAsync($"Key: {item} VALUE: {Environment.GetEnvironmentVariable(item)} ;");
                        }
                    }
                    await context.Response.WriteAsync($"Hello!");
                });

                //endpoints.MapHealthChecks("/health");
                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions()
                {
                    Predicate = (check) => check.Tags.Contains("ready"),
                });
               
                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions()
                {
                    Predicate = (_) => false
                });
                endpoints.MapControllers();
            });

            
            app.UseSwagger();
        }
    }
}
