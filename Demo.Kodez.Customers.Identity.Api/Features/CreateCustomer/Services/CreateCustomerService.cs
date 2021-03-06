using System.Threading.Tasks;
using Demo.Kodez.Customers.Identity.Api.Features.CreateCustomer.Models;
using Demo.Kodez.Customers.Identity.Api.Features.NewCustomerPromotion;
using Demo.Kodez.Customers.Identity.Api.Infrastructure.DataAccess;
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
        private readonly ILogger<CreateCustomerService> _logger;
        private readonly IValidator<CreateCustomerRequest> _validator;
        private readonly ICommandHandler<CreateCustomerCommand> _commandHandler;
        private readonly INewCustomerPromotionService _newCustomerPromotionService;

        public CreateCustomerService(IValidator<CreateCustomerRequest> validator, ICommandHandler<CreateCustomerCommand> commandHandler, 
            INewCustomerPromotionService newCustomerPromotionService, ILogger<CreateCustomerService> logger)
        {
            _validator = validator;
            _commandHandler = commandHandler;
            _newCustomerPromotionService = newCustomerPromotionService;
            _logger = logger;
        }

        public async Task<Result> CreateAsync(CreateCustomerRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Result.Failure(ErrorCodes.InvalidRequest, validationResult);
            }

            var command = new CreateCustomerCommand
            {
                CustomerId = request.CustomerId,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName
            };
            
            SetPropertiesToAdd();

            var operation = await _commandHandler.ExecuteAsync(command);

            await _newCustomerPromotionService.AddAsync(request);

            return operation;
        }
        
        private void SetPropertiesToAdd()
        {
            _commandHandler
                .Add(x => x.CustomerId)
                .Add(x => x.FirstName)
                .Add(x => x.LastName)
                .Add(x=>x.Email);
        }
    }
}