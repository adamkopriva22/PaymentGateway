using PaymentGateway.Core.Entities;

namespace PaymentGateway.Core.Interfaces
{
    public interface IPaymentService
    {
        Task<TransactionResult> AuthorizeAsync(AuthorizeTransaction transaction);
    }
}