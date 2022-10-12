using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PaymentGateway.API.Extensions
{
    public static class ModelStateExtensions
    {
        public static List<string> GetErrorMessages(this ModelStateDictionary modelStateDictionary)
        {
            if (modelStateDictionary != null)
            {
                return modelStateDictionary.Where(m => m.Value != null)
                    .SelectMany(m => m.Value!.Errors)
                    .Select(m => m.ErrorMessage)
                    .ToList();
            }

            return new List<string>();
        }
    }
}
