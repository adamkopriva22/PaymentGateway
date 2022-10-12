using Microsoft.EntityFrameworkCore;
using PaymentGateway.Core.Enums;

namespace PaymentGateway.Core.Entities
{
    [Index(nameof(TransactionKey), IsUnique = true)]
    public class Transaction : BaseEntity
    {
        public string TransactionKey { get; set; } = null!;
        public string? Currency { get; set; }
        public decimal? Amount { get; set; }
        public PaymentProviderType PaymentProvider { get; set; }
        public TransactionType TransactionType { get; set; }
        public TransactionStatus TransactionStatus { get; set; }
        public string? ProviderTransactionKey { get; set; }

        // TODO other fields like client id, order id, created at, updated at ....
    }
}
