namespace PaymentGateway.Core.Entities
{
    public class TransactionResult : ProcessResult
    {
        public TransactionResult(string? transactionKey = null, string? message = null)
        {
            TransactionKey = transactionKey;
            Success = transactionKey != null;
            Message = message;
        }

        public string? TransactionKey { get; set; }
    }
}
