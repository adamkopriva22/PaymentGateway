using Microsoft.Extensions.Logging;
using PaymentGateway.Core.Entities;
using PaymentGateway.Core.Extensions.Mappers;
using PaymentGateway.Core.Interfaces;

namespace PaymentGateway.Core.Services
{
    public class PaymentService : IPaymentService
    {
        public static readonly string AuthorizationSucceeded = "AUTHORIZATION_SUCCEEDED";
        public static readonly string AuthorizationFailed = "AUTHORIZATION_FAILED";

        private readonly ILogger<IPaymentService> logger;
        private readonly IPaymentProviderFactory paymentProviderFactory;
        private readonly IPaymentGatewayDbContext dbContext;

        public PaymentService(
            ILogger<IPaymentService> logger,
            IPaymentProviderFactory paymentProviderFactory,
            IPaymentGatewayDbContext dbContext)
        {
            this.logger = logger;
            this.paymentProviderFactory = paymentProviderFactory;
            this.dbContext = dbContext;
        }

        public async Task<TransactionResult> AuthorizeAsync(AuthorizeTransaction authorizeTransaction)
        {
            logger.LogDebug($"{nameof(PaymentService)}-{nameof(AuthorizeAsync)} - Authorizing transaction for {authorizeTransaction.PaymentProvider}");

            var paymentProvider = paymentProviderFactory.GetPaymentProvider(authorizeTransaction.PaymentProvider);

            // TODO
            // Here is crucial to make operation idempotent which was achived by idempotent key (transaction key) with compound key of client id and order it with unique index.
            //
            // Another thing to consider to make this more scalable would be to trigger background task to handle logic with payment provider.
            // In mean time we could return back to client link where status of payment can be checked.
            // Or let him set up some web hook which can be ping as soon as payment is processed.
            //
            // Also we would have to define how we are supposed to handle retries in case of payment failure. For example auto retries for transient errors and for other failures let client to decide.

            var transaction = authorizeTransaction.ToTransaction(Enums.TransactionStatus.Invoked);
            await dbContext.Transactions.AddAsync(transaction);
            await dbContext.SaveChangesAsync();

            var result = await paymentProvider.AuthorizeAsync(authorizeTransaction);

            transaction.TransactionStatus = result.Success ? Enums.TransactionStatus.Completed : Enums.TransactionStatus.Failed;
            transaction.ProviderTransactionKey = result.TransactionKey;
            await dbContext.SaveChangesAsync();

            return new TransactionResult(
                transactionKey: result.Success ? authorizeTransaction.TransactionKey : null,
                message: result.Success ? AuthorizationSucceeded : AuthorizationFailed);
        }
    }
}
