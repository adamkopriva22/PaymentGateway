using PaymentGateway.Core.Enums;

namespace PaymentGateway.Core.Interfaces
{
    public interface IPaymentProviderFactory
    {
        IPaymentProvider GetPaymentProvider(PaymentProviderType type);
    }
}