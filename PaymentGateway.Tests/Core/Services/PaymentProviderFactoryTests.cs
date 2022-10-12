using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Core.Entities;
using PaymentGateway.Core.Enums;
using PaymentGateway.Core.Interfaces;
using PaymentGateway.Core.Services;

namespace PaymentGateway.Tests.Core.Services
{
    public class PaymentProviderFactoryTests
    {
        public static IEnumerable<object[]> GetPaymentProviderTestData =>
            new List<object[]>
            {
                new object[] {
                    PaymentProviderType.Stripe
                },
                new object[] {
                    PaymentProviderType.Adyen
                }
            };

        [Theory]
        [MemberData(nameof(GetPaymentProviderTestData))]
        public void GIVEN_PaymentProviderFactory_WHEN_GetPaymentProvider_Then_CorrectServiceReturned(PaymentProviderType input)
        {
            // GIVEN
            ServiceProvider serviceProvider = GetMockServiceProvider();

            var paymentProviderFactory = new PaymentProviderFactory(serviceProvider);

            // WHEN
            var result = paymentProviderFactory.GetPaymentProvider(input);

            // THEN
            result.Should().NotBeNull();
            result.Type.Should().Be(input);
        }

        public static IEnumerable<object[]> GetPaymentProviderWithNonExistingServiceTestData =>
            new List<object[]>
            {
                new object[] {
                    (PaymentProviderType) int.MaxValue
                }
            };

        [Theory]
        [MemberData(nameof(GetPaymentProviderWithNonExistingServiceTestData))]
        public void GIVEN_PaymentProviderFactory_WHEN_GetPaymentProviderWithNonExistingService_Then_ExceptionThrown(PaymentProviderType input)
        {
            // GIVEN
            ServiceProvider serviceProvider = GetMockServiceProvider();

            var PaymentProviderFactory = new PaymentProviderFactory(serviceProvider);

            // WHEN
            Action act = () => PaymentProviderFactory.GetPaymentProvider(input);

            // THEN
            act.Should().Throw<NotImplementedException>();
        }

        private static ServiceProvider GetMockServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IPaymentProvider, MockGateway1>();
            serviceCollection.AddTransient<IPaymentProvider, MockGateway2>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider;
        }
    }

    public class MockGateway1 : IPaymentProvider
    {
        public PaymentProviderType Type => PaymentProviderType.Stripe;

        public Task<TransactionResult> AuthorizeAsync(AuthorizeTransaction transaction) => Task.FromResult(new TransactionResult());

        public Task<TransactionResult> CaptureAsync(CaptureTransaction transaction) => Task.FromResult(new TransactionResult());

        public Task<TransactionResult> VoidAsync(VoidTransaction transaction) => Task.FromResult(new TransactionResult());
    }

    public class MockGateway2 : IPaymentProvider
    {
        public PaymentProviderType Type => PaymentProviderType.Adyen;

        public Task<TransactionResult> AuthorizeAsync(AuthorizeTransaction transaction) => Task.FromResult(new TransactionResult());

        public Task<TransactionResult> CaptureAsync(CaptureTransaction transaction) => Task.FromResult(new TransactionResult());

        public Task<TransactionResult> VoidAsync(VoidTransaction transaction) => Task.FromResult(new TransactionResult());
    }

}
