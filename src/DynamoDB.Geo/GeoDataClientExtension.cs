using DynamoDB.Geo.Contract;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DynamoDB.Geo
{
    public static class GeoDataClientExtension
    {
        public static IServiceCollection AddGeoDataClient(
            this IServiceCollection services,
            Action<GeoDataClientOptions> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            services.AddOptions();
            services.Configure(setupAction);
            services.AddSingleton<IGeoDataClient, GeoDataClient>();

            return services;
        }
    }
}
