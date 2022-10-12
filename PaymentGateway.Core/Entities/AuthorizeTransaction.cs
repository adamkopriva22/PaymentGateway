using PaymentGateway.Core.Enums;

namespace PaymentGateway.Core.Entities
{
    public class AuthorizeTransaction
    {
        public string TransactionKey { get; set; } = null!;
        public string? Currency { get; set; }
        public decimal? Amount { get; set; }
        public PaymentProviderType PaymentProvider { get; set; }
    }    
}
