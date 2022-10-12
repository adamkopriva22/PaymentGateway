namespace PaymentGateway.Core.Entities
{
    public class ProcessResult
    {
        public ProcessResult()
        {
            Success = true;
        }

        public ProcessResult(string? message)
        {
            Success = false;
            Message = message;
        }

        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
