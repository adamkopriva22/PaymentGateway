using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.API.Extensions;
using PaymentGateway.API.Extensions.Mappers;
using PaymentGateway.API.Models;
using PaymentGateway.Core.Interfaces;

namespace PaymentGateway.API.Controllers
{
    [Route("api/payments")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService paymentService;

        public PaymentController(
            IPaymentService paymentService)
        {
            this.paymentService = paymentService;
        }

        [HttpPost("authorize")]
        public async Task<IActionResult> AuthorizeAsync([FromBody] AuthorizePaymentModel paymentModel)
        {
            // TODO
            // add other validation like correct client id, amount range, currency value, etc. ...
            // add exception handler middle ware to translate exceptions to some common error response

            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());

            var result = await paymentService.AuthorizeAsync(paymentModel.ToTransaction());

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.TransactionKey);
        }

        [HttpPost("capture")]
        public Task<IActionResult> CaptureAsync([FromBody] CapturePaymentModel paymentModel) =>
            // TODO similar as for authorize
            throw new NotImplementedException();


        [HttpPost("void")]
        public Task<IActionResult> VoidAsync([FromBody] VoidPaymentModel paymentModel) =>
            // TODO similar as for authorize
            throw new NotImplementedException();
    }
}