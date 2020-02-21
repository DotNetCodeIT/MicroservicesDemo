using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using ShopWebSite.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShopWebSite.Services;
using Polly.Extensions.Http;
using Polly;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using ShopWebSite.Models;

namespace ShopWebSite
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddHttpClient<IApiService<Supplier>, ApiService<Supplier>>(client =>
            {
                client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("API_SUPPLIER_ADDRESS"));
            })
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetCircuitBreakerPolicy());

            services.AddHttpClient<IApiService<Customer>, ApiService<Customer>>(client =>
            {
                client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("API_CUSTOMER_ADDRESS"));
            })
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetCircuitBreakerPolicy());

            services.AddHttpClient<IApiService<Product>, ApiService<Product>>(client =>
            {
                client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("API_PRODUCT_ADDRESS"));
            })
             .AddPolicyHandler(GetRetryPolicy())
             .AddPolicyHandler(GetCircuitBreakerPolicy());

            services.AddHttpClient<IApiService<ProductPrice>, ApiService<ProductPrice>>(client =>
            {
                client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("API_PRODUCTPRICE_ADDRESS"));
            })
             .AddPolicyHandler(GetRetryPolicy())
             .AddPolicyHandler(GetCircuitBreakerPolicy());


            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

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
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "Area",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                  name: "default",
                  pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
            });
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
        static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        }
    }
}
