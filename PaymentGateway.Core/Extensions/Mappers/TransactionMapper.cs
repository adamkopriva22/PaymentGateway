using PaymentGateway.Core.Entities;
using PaymentGateway.Core.Enums;

namespace PaymentGateway.Core.Extensions.Mappers
{
    public static class TransactionMapper
    {
        public static Transaction ToTransaction(this AuthorizeTransaction authorizeTransaction, TransactionStatus status) => new Transaction
        {
            TransactionKey = authorizeTransaction.TransactionKey,
            Currency = authorizeTransaction.Currency,
            Amount = authorizeTransaction.Amount,
            PaymentProvider = authorizeTransaction.PaymentProvider,
            TransactionType = TransactionType.Authorize,
            TransactionStatus = status
        };
    }
}
