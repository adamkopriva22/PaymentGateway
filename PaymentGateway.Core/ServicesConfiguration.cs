using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Core.Interfaces;
using PaymentGateway.Core.Services;

namespace PaymentGateway.Core
{
    public static class ServicesConfiguration
    {
        public static void AddCoreServices(this IServiceCollection services)
        {
            services.AddTransient<IPaymentService, PaymentService>();
            services.AddTransient<IPaymentProviderFactory, PaymentProviderFactory>(); 
            services.AddTransient<IIdentityProvider, IdentityProvider>();
        }
    }
}
