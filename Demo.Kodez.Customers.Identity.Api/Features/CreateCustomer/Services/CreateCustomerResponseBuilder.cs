using Demo.Kodez.Customers.Identity.Api.Features.CreateCustomer.Models;
using Demo.Kodez.Customers.Identity.Api.Shared;
using Demo.Kodez.Customers.Identity.Api.Shared.Services;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Kodez.Customers.Identity.Api.Features.CreateCustomer.Services
{
    public class CreateCustomerResponseBuilder : IResponseBuilder<CreateCustomerRequest, Result>
    {
        public IActionResult GetResponse(CreateCustomerRequest request, Result operation)
        {
            return new OkResult();
        }
    }
}