using System.Linq;
using System.Net;
using Demo.Kodez.Customers.Identity.Api.Features.UpdateCustomer.Models;
using Demo.Kodez.Customers.Identity.Api.Shared;
using Demo.Kodez.Customers.Identity.Api.Shared.Constants;
using Demo.Kodez.Customers.Identity.Api.Shared.Services;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Kodez.Customers.Identity.Api.Features.UpdateCustomer.Services
{
    public class UpdateCustomerResponseBuilder : IResponseBuilder<UpdateCustomerRequest, Result>
    {
        public IActionResult GetResponse(UpdateCustomerRequest request, Result operation)
        {
            if (operation.Status)
            {
                return new OkResult();
            }
        
            return GetErrorResponse(operation);
        }
        
        private IActionResult GetErrorResponse(Result operation)
        {
            var errorResponse = new
            {
                operation.ErrorCode,
                Errors = operation.ValidationResult.Errors.Select(x =>
                    new
                    {
                        x.PropertyName,
                        x.ErrorMessage
                    })
            };
            return operation.ErrorCode switch
            {
                ErrorCodes.InvalidRequest => new BadRequestObjectResult(errorResponse),
                _ => new BadRequestObjectResult(errorResponse)
            };
        }
    }
}