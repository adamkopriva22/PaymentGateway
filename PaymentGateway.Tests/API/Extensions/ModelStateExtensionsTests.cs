using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PaymentGateway.API.Extensions;

namespace PaymentGateway.Tests.API.Extensions
{
    public class ModelStateExtensionsTests
    {
        public static IEnumerable<object[]> GetErrorMessagesTestData =>
            new List<object[]>
            {
                new object[] {
                    new Dictionary<string, string>
                    {
                        { "1", "Error 1" }
                    },
                    new List<string>
                    {
                        { "Error 1" }
                    }
                },
                new object[] {
                    new Dictionary<string, string>
                    {
                        { "1", "Error 1" },
                        { "2", "Error 2" }
                    },
                    new List<string>
                    {
                        { "Error 1" },
                        { "Error 2" }
                    }
                },
                new object[] {
                    new Dictionary<string, string>
                    {
                        { "1", "Error 1" },
                        { "2", "Error 2" },
                        { "3", "Error 3" },
                        { "4", "Error 4" },
                        { "5", "Error 5" },
                        { "6", "Error 6" }
                    },
                    new List<string>
                    {
                        { "Error 1" },
                        { "Error 2" },
                        { "Error 3" },
                        { "Error 4" },
                        { "Error 5" },
                        { "Error 6" }
                    }
                }
            };

        [Theory]
        [MemberData(nameof(GetErrorMessagesTestData))]
        public void GIVEN_ModelStateDictionary_WHEN_GetErrorMessages_Then_CorrectValueReturned(Dictionary<string, string> input, List<string> expectedValue)
        {
            // GIVEN
            var modelState = new ModelStateDictionary();

            foreach (var error in input)
            {
                modelState.AddModelError(error.Key, error.Value);
            }

            // WHEN
            var result = modelState.GetErrorMessages();

            // THEN
            result.Should().BeEquivalentTo(expectedValue);
        }

        [Fact]
        public void GIVEN_ModelStateDictionary_WHEN_GetErrorMessagesWithDictAsNull_Then_CorrectValueReturned()
        {
            // GIVEN
            var modelState = new ModelStateDictionary();

            // WHEN
            var result = modelState.GetErrorMessages();

            // THEN
            result.Should().BeEquivalentTo(new List<string>());
        }
    }
}
