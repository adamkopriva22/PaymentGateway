using PaymentGateway.Core.Entities;
using PaymentGateway.Core.Enums;

namespace PaymentGateway.Core.Interfaces
{
    public interface IPaymentProvider
    {
        PaymentProviderType Type {get;}
        Task<TransactionResult> AuthorizeAsync(AuthorizeTransaction transaction);
        Task<TransactionResult> CaptureAsync(CaptureTransaction transaction);
        Task<TransactionResult> VoidAsync(VoidTransaction transaction);

    }
}
