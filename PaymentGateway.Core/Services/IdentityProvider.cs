using PaymentGateway.Core.Entities;
using PaymentGateway.Core.Interfaces;

namespace PaymentGateway.Core.Services
{
    public class IdentityProvider : IIdentityProvider
    {
        public Client GetClient(string clientId, string clientKey)
        {
            // TODO implement client provider 
            return new Client
            {
                Id = Guid.NewGuid().ToString()
            };
        }
    }
}
