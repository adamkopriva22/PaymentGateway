using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Core.Enums;
using PaymentGateway.Core.Interfaces;

namespace PaymentGateway.Core.Services
{
    public class PaymentProviderFactory : IPaymentProviderFactory
    {
        private readonly IServiceProvider serviceProvider;

        public PaymentProviderFactory(
            IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IPaymentProvider GetPaymentProvider(PaymentProviderType type)
        {
            var paymentProvider = serviceProvider.GetServices<IPaymentProvider>()
                .FirstOrDefault(s => s.Type == type);

            if (paymentProvider == null)
            {
                throw new NotImplementedException($"No implementation for payment provider with type {type} found!");
            }

            return paymentProvider;
        }
    }
}
