using FluentAssertions;
using PaymentGateway.API.Extensions.Mappers;
using PaymentGateway.API.Models;
using PaymentGateway.Core.Entities;
using PaymentGateway.Core.Enums;

namespace TaskBilling.Tests.API.Extensions.Mappers
{
    public class PaymentMapperTests
    {
        public static IEnumerable<object[]> ToTransactionTestData =>
            new List<object[]>
            {
                new object[] {
                   new AuthorizePaymentModel
                    {
                        ClientId = "123",
                        OrderId = "AB_123456",
                        Currency = "USD",
                        Amount = (decimal) 200.0,
                        PaymentProvider = PaymentProviderType.Stripe
                    },
                    new AuthorizeTransaction
                    {
                        Currency = "USD",
                        Amount = (decimal) 200.0,
                        PaymentProvider = PaymentProviderType.Stripe,
                        TransactionKey = "123-AB_123456"
                    }
                },
                new object[] {
                     new AuthorizePaymentModel
                    {
                        ClientId = "123",
                        OrderId = "AB_123456",
                        Currency = "USD",
                        Amount = (decimal) 0.0,
                        PaymentProvider = PaymentProviderType.Adyen
                    },
                    new AuthorizeTransaction
                    {
                        Currency = "USD",
                        Amount = (decimal) 0.0,
                        PaymentProvider = PaymentProviderType.Adyen,
                        TransactionKey = "123-AB_123456"
                    }
                }
            };

        [Theory]
        [MemberData(nameof(ToTransactionTestData))]
        public void GIVEN_AuthorizePaymentModel_WHEN_ToTransaction_Then_CorrectlyMapped(AuthorizePaymentModel input, AuthorizeTransaction expectedValue)
        {
            // GIVEN
            // WHEN
            var result = input.ToTransaction();

            // THEN
            result.Should().BeEquivalentTo(expectedValue);
        }
    }
}
