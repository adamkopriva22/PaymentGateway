using PaymentGateway.Core.Entities;

namespace PaymentGateway.Core.Interfaces
{
    public interface IIdentityProvider
    {
        Client GetClient(string clientId, string clientKey);
    }
}