using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaymentGateway.Core.Entities;
using PaymentGateway.Core.Enums;
using PaymentGateway.Core.Interfaces;
using PaymentGateway.Core.Services;
using Stripe;

namespace PaymentGateway.Infrastructure.Providers.Stripe
{
    public class StripePaymentProvider : IPaymentProvider
    {
        private readonly ILogger<StripePaymentProvider> logger;

        public StripePaymentProvider(
            ILogger<StripePaymentProvider> logger,
            IOptions<StripePaymentConfig> options)
        {
            this.logger = logger;
            StripeConfiguration.ApiKey = options.Value.ApiKey;
        }

        public PaymentProviderType Type => PaymentProviderType.Stripe;

        public async Task<TransactionResult> AuthorizeAsync(AuthorizeTransaction transaction)
        {
            logger.LogDebug($"{nameof(StripePaymentProvider)}-{nameof(AuthorizeAsync)} - Authorizing transaction for Stripe");

            try
            {
                var options = new ChargeCreateOptions
                {
                    Amount = ConvertAmount(transaction.Amount!.Value),
                    Currency = transaction.Currency,
                };
                var service = new ChargeService();
                var result = await service.CreateAsync(options);

                return new TransactionResult(transactionKey: result.Id);
            }
            catch (Exception ex) 
            {
                logger.LogError($"{nameof(StripePaymentProvider)}-{nameof(AuthorizeAsync)} - Authorizing transaction for Stripe failed with exception: ", ex);
                return new TransactionResult(null, PaymentService.AuthorizationFailed);
            }
        }

        private long? ConvertAmount(decimal amount) =>
            // TODO handle rounding
            (long)amount * 100;


        public Task<TransactionResult> CaptureAsync(CaptureTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public Task<TransactionResult> VoidAsync(VoidTransaction transaction)
        {
            throw new NotImplementedException();
        }
    }
}
