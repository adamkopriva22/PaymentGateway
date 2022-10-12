namespace PaymentGateway.Infrastructure.Providers.Stripe
{
    public class StripePaymentConfig
    {
        public const string SettingsKey = "StripePayment";
        public string ApiKey { get; set; } = null!;
    }
}