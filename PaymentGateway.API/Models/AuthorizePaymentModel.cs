using PaymentGateway.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.API.Models
{
    public class AuthorizePaymentModel
    {
        [Required(ErrorMessage = "REQUIRED_FIELD_MISSING_CURRENCY")]
        public string? Currency { get; set; }

        [Required(ErrorMessage = "REQUIRED_FIELD_MISSING_AMOUNT")]
        public decimal? Amount { get; set; }

        [Required(ErrorMessage = "REQUIRED_FIELD_MISSING_ORDERID")]
        public string? OrderId { get; set; }

        [Required(ErrorMessage = "REQUIRED_FIELD_MISSING_CLIENTID")]
        public string? ClientId { get; set; }

        [Required(ErrorMessage = "REQUIRED_FIELD_PAYMENT_PROVIDER")]
        [EnumDataType(typeof(PaymentProviderType), ErrorMessage = "INVALID_PAYMENT_PROVIDER")]
        public PaymentProviderType? PaymentProvider { get; set; }
    }
}
