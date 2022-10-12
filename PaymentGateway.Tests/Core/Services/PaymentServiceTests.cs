using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using PaymentGateway.Core.Entities;
using PaymentGateway.Core.Enums;
using PaymentGateway.Core.Interfaces;
using PaymentGateway.Core.Services;
using PaymentGateway.Infrastructure.Db;

namespace PaymentGateway.Tests.Core.Services
{
    public class PaymentServiceTests
    {
        public static IEnumerable<object[]> AuthorizeAsyncTestData =>
            new List<object[]>
            {
                new object[] {
                    new AuthorizeTransaction
                    {
                        Currency = "USD",
                        Amount = (decimal) 200.0,
                        PaymentProvider = PaymentProviderType.Stripe,
                        TransactionKey = "123-AB_123456"
                    },
                    new TransactionResult()
                    {
                        Success = true,
                        TransactionKey = "123-AB_123456",
                        Message = "AUTHORIZATION_SUCCEEDED"
                    },
                    new Transaction
                    {
                        Currency = "USD",
                        Amount = (decimal) 200.0,
                        PaymentProvider = PaymentProviderType.Stripe,
                        TransactionKey = "123-AB_123456",
                        ProviderTransactionKey = "PROVIDER_TRANSACTION_KEY",
                        TransactionStatus = TransactionStatus.Completed,
                        TransactionType = TransactionType.Authorize
                    }
                },
                new object[] {
                    new AuthorizeTransaction
                    {
                        Currency = "USD",
                        Amount = (decimal) 200.0,
                        PaymentProvider = PaymentProviderType.Adyen,
                        TransactionKey = "123-AB_123456"
                    },
                    new TransactionResult()
                    {
                        Success = true,
                        TransactionKey = "123-AB_123456",
                        Message = "AUTHORIZATION_SUCCEEDED"
                    },
                    new Transaction
                    {
                        Currency = "USD",
                        Amount = (decimal) 200.0,
                        PaymentProvider = PaymentProviderType.Adyen,
                        TransactionKey = "123-AB_123456",
                        ProviderTransactionKey = "PROVIDER_TRANSACTION_KEY",
                        TransactionStatus = TransactionStatus.Completed,
                        TransactionType = TransactionType.Authorize
                    }
                },
            };

        [Theory]
        [MemberData(nameof(AuthorizeAsyncTestData))]
        public async Task GIVEN_PaymentService_WHEN_AuthorizeAsync_Then_CorrectValueReturned(AuthorizeTransaction input, TransactionResult expectedResult, Transaction transactionResult)
        {
            // GIVEN
            var mockPaymentProviderFactory = new Mock<IPaymentProviderFactory>();

            mockPaymentProviderFactory
                .Setup(m => m.GetPaymentProvider(It.IsAny<PaymentProviderType>()))
                .Returns(new MockGatewaySuccess());

            var dbContext = GetInMemoryDbContext();

            var paymentService = new PaymentService(
                new Mock<ILogger<PaymentService>>().Object,
                mockPaymentProviderFactory.Object,
                dbContext
                );

            // WHEN
            var result = await paymentService.AuthorizeAsync(input);

            // THEN
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);

            var transaction = dbContext.Transactions.Single();
            transaction.Should().BeEquivalentTo(transactionResult, options => options.Excluding(r => r.Id));
        }

        public static IEnumerable<object[]> AuthorizeAsyncWithFailure =>
            new List<object[]>
            {
                new object[] {
                     new AuthorizeTransaction
                    {
                        Currency = "USD",
                        Amount = (decimal) 200.0,
                        PaymentProvider = PaymentProviderType.Stripe,
                        TransactionKey = "123-AB_123456"
                    },
                    new TransactionResult()
                    {
                        Success = false,
                        Message = "AUTHORIZATION_FAILED"
                    },
                    new Transaction
                    {
                        Currency = "USD",
                        Amount = (decimal) 200.0,
                        PaymentProvider = PaymentProviderType.Stripe,
                        TransactionKey = "123-AB_123456",
                        TransactionStatus = TransactionStatus.Failed,
                        TransactionType = TransactionType.Authorize
                    }
                }
            };

        [Theory]
        [MemberData(nameof(AuthorizeAsyncWithFailure))]
        public async Task GIVEN_PaymentService_WHEN_AuthorizeAsyncWithFailure_Then_CorrectValueReturned(AuthorizeTransaction input, TransactionResult expectedResult, Transaction transactionResult)
        {
            // GIVEN
            var mockPaymentProviderFactory = new Mock<IPaymentProviderFactory>();

            mockPaymentProviderFactory
               .Setup(m => m.GetPaymentProvider(It.IsAny<PaymentProviderType>()))
               .Returns(new MockGatewayFailure());

            var dbContext = GetInMemoryDbContext();

            var paymentService = new PaymentService(
                new Mock<ILogger<PaymentService>>().Object,
                mockPaymentProviderFactory.Object,
                dbContext
                );

            // WHEN
            var result = await paymentService.AuthorizeAsync(input);

            // THEN
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);

            var transaction = dbContext.Transactions.Single();
            transaction.Should().BeEquivalentTo(transactionResult, options => options.Excluding(r => r.Id));
        }

        private PaymentGatewayDbContext GetInMemoryDbContext() => 
            new PaymentGatewayDbContext(new DbContextOptionsBuilder<PaymentGatewayDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString(), new InMemoryDatabaseRoot())
                .Options);
    }

    public class MockGatewaySuccess : IPaymentProvider
    {
        public PaymentProviderType Type => (PaymentProviderType)int.MaxValue;

        public Task<TransactionResult> AuthorizeAsync(AuthorizeTransaction transaction) => Task.FromResult(new TransactionResult("PROVIDER_TRANSACTION_KEY"));

        public Task<TransactionResult> CaptureAsync(CaptureTransaction transaction) => Task.FromResult(new TransactionResult("PROVIDER_TRANSACTION_KEY"));

        public Task<TransactionResult> VoidAsync(VoidTransaction transaction) => Task.FromResult(new TransactionResult("PROVIDER_TRANSACTION_KEY"));
    }


    public class MockGatewayFailure : IPaymentProvider
    {
        public PaymentProviderType Type => (PaymentProviderType)int.MaxValue;

        public Task<TransactionResult> AuthorizeAsync(AuthorizeTransaction transaction) => Task.FromResult(new TransactionResult(null, "AUTHORIZATION_FAILED"));

        public Task<TransactionResult> CaptureAsync(CaptureTransaction transaction) => Task.FromResult(new TransactionResult(null, "AUTHORIZATION_FAILED"));

        public Task<TransactionResult> VoidAsync(VoidTransaction transaction) => Task.FromResult(new TransactionResult(null, "AUTHORIZATION_FAILED"));
    }
    
}
