using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Core.Interfaces;
using PaymentGateway.Infrastructure.Db;
using PaymentGateway.Infrastructure.Providers.Stripe;

namespace PaymentGateway.Infrastructure
{
    public static class ServicesConfiguration
    {
        public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IPaymentProvider, StripePaymentProvider>();
            services.Configure<StripePaymentConfig>(configuration.GetSection(StripePaymentConfig.SettingsKey));

            services.AddDbContext<PaymentGatewayDbContext>
                (o => o.UseInMemoryDatabase("PaymentGateway"));
            services.AddScoped<IPaymentGatewayDbContext>(provider => provider.GetService<PaymentGatewayDbContext>()!);
        }
    }
}
