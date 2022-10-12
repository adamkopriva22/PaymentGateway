using PaymentGateway.API.Models;
using PaymentGateway.Core.Entities;

namespace PaymentGateway.API.Extensions.Mappers
{
    public static class PaymentMapper
    {
        public static AuthorizeTransaction ToTransaction(this AuthorizePaymentModel authorizePaymentModel) => new AuthorizeTransaction
        {
            Currency = authorizePaymentModel.Currency,
            Amount = authorizePaymentModel.Amount,
            PaymentProvider = authorizePaymentModel.PaymentProvider!.Value,
            TransactionKey = $"{authorizePaymentModel.ClientId}-{authorizePaymentModel.OrderId}"
        };
    }
}
