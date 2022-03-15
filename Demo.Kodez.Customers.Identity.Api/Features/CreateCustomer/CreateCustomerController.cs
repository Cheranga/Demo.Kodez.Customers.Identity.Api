using System.Threading.Tasks;
using Demo.Kodez.Customers.Identity.Api.Features.CreateCustomer.Models;
using Demo.Kodez.Customers.Identity.Api.Features.CreateCustomer.Services;
using Demo.Kodez.Customers.Identity.Api.Shared;
using Demo.Kodez.Customers.Identity.Api.Shared.Services;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Kodez.Customers.Identity.Api.Features.CreateCustomer
{
    public class CreateCustomerController : ControllerBase
    {
        private readonly ICreateCustomerService _createCustomerService;
        private readonly IResponseBuilder<CreateCustomerRequest, Result> _responseBuilder;

        public CreateCustomerController(ICreateCustomerService createCustomerService, IResponseBuilder<CreateCustomerRequest, Result> responseBuilder)
        {
            _createCustomerService = createCustomerService;
            _responseBuilder = responseBuilder;
        }

        [HttpPost("api/customers")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateCustomerRequest request)
        {
            var operation = await _createCustomerService.CreateAsync(request);
            var response = _responseBuilder.GetResponse(request, operation);

            return response;
        }
    }
}