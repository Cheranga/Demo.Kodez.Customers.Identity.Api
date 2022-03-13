using System.Threading.Tasks;
using Demo.Kodez.Customers.Identity.Api.Features.CreateCustomer.Models;
using Demo.Kodez.Customers.Identity.Api.Shared;
using Demo.Kodez.Customers.Identity.Api.Shared.Constants;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Demo.Kodez.Customers.Identity.Api.Features.CreateCustomer.Services
{
    public interface ICreateCustomerService
    {
        Task<Result> CreateAsync(CreateCustomerRequest request);
    }
    
    public class CreateCustomerService : ICreateCustomerService
    {
        private readonly IValidator<CreateCustomerRequest> _validator;
        private readonly ILogger<CreateCustomerService> _logger;

        public CreateCustomerService(IValidator<CreateCustomerRequest> validator, ILogger<CreateCustomerService> logger)
        {
            _validator = validator;
            _logger = logger;
        }
        
        public async Task<Result> CreateAsync(CreateCustomerRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Result.Failure(ErrorCodes.InvalidRequest, validationResult);
            }

            return Result.Success();
        }
    }
}