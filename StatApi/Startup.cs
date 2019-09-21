using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using StatApi.Data;

namespace StatApi
{
    public class Startup
    {
        private static readonly string VERSION = "v1.0";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TwitchDbContext>(builder =>
            {
                builder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(VERSION, new OpenApiInfo
                {
                    Title = "Twitch Analysis Api",
                    Version = VERSION,
                    Contact = new OpenApiContact
                    {
                        Name = "Xander Stoffels",
                        Email = "xander.s@outlook.com",
                        Url = new Uri("https://github.com/XanderStoffels/twitch-analyser")
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", $"Twitch Analysis Api {VERSION}"); });

            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}