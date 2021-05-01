using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Glue.Delivery.WebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOptions<TOption>(this IServiceCollection services,
            IConfiguration configuration) where TOption : class
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services.AddOptions<TOption>()
                .Bind(configuration.GetSection(typeof(TOption).Name))
                .ValidateDataAnnotations();

            return services;
        }
    }
}