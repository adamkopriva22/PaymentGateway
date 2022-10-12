using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PaymentGateway.API.Controllers;
using PaymentGateway.API.Models;
using PaymentGateway.Core.Entities;
using PaymentGateway.Core.Enums;
using PaymentGateway.Core.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Tests.API.Controllers
{
    public class PaymentControllerTest
    {
        public static IEnumerable<object[]> AuthorizeAsyncTestData =>
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
                    new TransactionResult()
                    {
                        Success = true,
                        TransactionKey = "123-AB_123456"
                    },
                    "123-AB_123456"
                },
                new object[] {
                    new AuthorizePaymentModel
                    {
                        ClientId = "123",
                        OrderId = "AB_123456",
                        Currency = "USD",
                        Amount = (decimal) 200.0,
                        PaymentProvider = PaymentProviderType.Adyen
                    },
                    new TransactionResult()
                    {
                        Success = true,
                        TransactionKey = "123-AB_123456"
                    },
                    "123-AB_123456"
                }
            };

        [Theory]
        [MemberData(nameof(AuthorizeAsyncTestData))]
        public async Task GIVEN_PaymentController_WHEN_AuthorizeAsync_Then_CorrectValueReturned(AuthorizePaymentModel input, TransactionResult expectedTransactionResult, string expectedResult)
        {
            // GIVEN
            var mockPaymentService = new Mock<IPaymentService>();

            mockPaymentService
                .Setup(m => m.AuthorizeAsync(It.IsAny<AuthorizeTransaction>()))
                .Returns(Task.FromResult(expectedTransactionResult));

            var controller = new PaymentController(mockPaymentService.Object);
            MockModelState(input, controller);

            // WHEN
            var result = await controller.AuthorizeAsync(input);

            // THEN
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(new OkObjectResult(expectedResult));
        }

        public static IEnumerable<object[]> AuthorizeAsyncModelStateValidationTestData =>
            new List<object[]>
            {
                new object[] {
                    new AuthorizePaymentModel
                    {
                    },
                    new List<string>
                    {
                        { "REQUIRED_FIELD_MISSING_CURRENCY" },
                        { "REQUIRED_FIELD_MISSING_AMOUNT" },
                        { "REQUIRED_FIELD_MISSING_ORDERID" },
                        { "REQUIRED_FIELD_MISSING_CLIENTID" },
                        { "REQUIRED_FIELD_PAYMENT_PROVIDER" }
                    }
                },
                new object[] {
                    new AuthorizePaymentModel
                    {
                        Currency = "USD"
                    },
                    new List<string>
                    {
                        { "REQUIRED_FIELD_MISSING_AMOUNT" },
                        { "REQUIRED_FIELD_MISSING_ORDERID" },
                        { "REQUIRED_FIELD_MISSING_CLIENTID" },
                        { "REQUIRED_FIELD_PAYMENT_PROVIDER" }
                    }
                },
                new object[] {
                    new AuthorizePaymentModel
                    {
                         Currency = "USD",
                         Amount = (decimal) 200.0
                    },
                    new List<string>
                    {
                        { "REQUIRED_FIELD_MISSING_ORDERID" },
                        { "REQUIRED_FIELD_MISSING_CLIENTID" },
                        { "REQUIRED_FIELD_PAYMENT_PROVIDER" }
                    }
                },
                new object[] {
                    new AuthorizePaymentModel
                    {
                        Currency = "USD",
                        Amount = (decimal) 200.0,
                        OrderId = "AB_123456",
                    },
                    new List<string>
                    {
                        { "REQUIRED_FIELD_MISSING_CLIENTID" },
                        { "REQUIRED_FIELD_PAYMENT_PROVIDER" }
                    }
                },
                new object[] {
                    new AuthorizePaymentModel
                    {
                        Currency = "USD",
                        Amount = (decimal) 200.0,
                        OrderId = "AB_123456",
                        ClientId = "123",
                    },
                    new List<string>
                    {
                        { "REQUIRED_FIELD_PAYMENT_PROVIDER" }
                    }
                },
                new object[] {
                    new AuthorizePaymentModel
                    {
                        Currency = "USD",
                        Amount = (decimal) 200.0,
                        OrderId = "AB_123456",
                        ClientId = "123",
                        PaymentProvider = (PaymentProviderType) int.MaxValue
                    },
                    new List<string>
                    {
                        { "INVALID_PAYMENT_PROVIDER" }
                    }
                }
            };

        [Theory]
        [MemberData(nameof(AuthorizeAsyncModelStateValidationTestData))]
        public async Task GIVEN_PaymentController_WHEN_AuthorizeAsyncWithInvalidInputData_Then_CorrectValueReturned(AuthorizePaymentModel input, List<string> errors)
        {
            // GIVEN
            var controller = new PaymentController(new Mock<IPaymentService>().Object);
            MockModelState(input, controller);

            // WHEN
            var result = await controller.AuthorizeAsync(input);

            // THEN
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(new BadRequestObjectResult(errors));
        }

        public static IEnumerable<object[]> CreateOrderAsyncWithUnsucesfullBillingServiceResultTestData =>
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
                    new TransactionResult()
                    {
                        Success = false,
                        Message = "AUTHORIZAION_FAILED"
                    },
                    "AUTHORIZAION_FAILED"
                }
            };

        [Theory]
        [MemberData(nameof(CreateOrderAsyncWithUnsucesfullBillingServiceResultTestData))]
        public async Task GIVEN_PaymentController_WHEN_AuthorizeAsyncWithUnsucesfullPaymentServiceResult_Then_CorrectValueReturned(AuthorizePaymentModel input, TransactionResult expectedTransactionResult, string expectedMessage)
        {

            // GIVEN
            var mockPaymentService = new Mock<IPaymentService>();

            mockPaymentService
                .Setup(m => m.AuthorizeAsync(It.IsAny<AuthorizeTransaction>()))
                .Returns(Task.FromResult(expectedTransactionResult));

            var controller = new PaymentController(mockPaymentService.Object);
            MockModelState(input, controller);

            // WHEN
            var result = await controller.AuthorizeAsync(input);

            // THEN
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(new BadRequestObjectResult(expectedMessage));
        }

        private void MockModelState<TModel, TController>(TModel model, TController controller) where TController : ControllerBase
        {
            var validationContext = new ValidationContext(model!, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(model!, validationContext, validationResults, true);

            foreach (var validationResult in validationResults)
            {
                controller.ModelState.AddModelError(validationResult.MemberNames.First(), validationResult.ErrorMessage!);
            }
        }
    }
}
