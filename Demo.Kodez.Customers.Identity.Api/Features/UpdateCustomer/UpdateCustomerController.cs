using System.Threading.Tasks;
using Demo.Kodez.Customers.Identity.Api.Features.UpdateCustomer.Models;
using Demo.Kodez.Customers.Identity.Api.Features.UpdateCustomer.Services;
using Demo.Kodez.Customers.Identity.Api.Shared;
using Demo.Kodez.Customers.Identity.Api.Shared.Services;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Kodez.Customers.Identity.Api.Features.UpdateCustomer
{
    public class UpdateCustomerController : ControllerBase
    {
        private readonly IResponseBuilder<UpdateCustomerRequest, Result> _responseBuilder;
        private readonly IUpdateCustomerService _updateCustomerService;

        public UpdateCustomerController(IUpdateCustomerService updateCustomerService, IResponseBuilder<UpdateCustomerRequest, Result> responseBuilder)
        {
            _updateCustomerService = updateCustomerService;
            _responseBuilder = responseBuilder;
        }

        [HttpPut("api/customers/{customerId}")]
        public async Task<IActionResult> UpdateAsync([FromRoute]string customerId, [FromBody] UpdateCustomerRequest request)
        {
            request ??= new UpdateCustomerRequest();
            request.CustomerId = customerId;
            
            var operation = await _updateCustomerService.UpdateAsync(request);
            var response = _responseBuilder.GetResponse(request, operation);

            return response;
        }
    }
}