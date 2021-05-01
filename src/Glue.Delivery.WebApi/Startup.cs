using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using Glue.Delivery.Core.Dto;
using Glue.Delivery.Core.Profiles;
using Glue.Delivery.Core.Stores;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Glue.Delivery.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(opts =>
            {
                opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Glue.Delivery.API", Version = "v1"});
            });

            services.AddMediatR(typeof(OrderDeliveryDto));
            services
                .AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<DeliveryDbContext>(
                    (sp, options) => { options.UseInMemoryDatabase("DeliveryDB").UseInternalServiceProvider(sp); },
                    ServiceLifetime.Singleton);

            services.AddSingleton(new MapperConfiguration(m => m.AddProfile<DeliveryProfile>()).CreateMapper());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Glue.Delivery.API v1"));
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}