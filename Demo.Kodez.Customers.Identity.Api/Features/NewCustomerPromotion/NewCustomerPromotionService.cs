using System.Threading.Tasks;
using Demo.Kodez.Customers.Identity.Api.Features.CreateCustomer.Models;
using Demo.Kodez.Customers.Identity.Api.Infrastructure.DataAccess;
using Demo.Kodez.Customers.Identity.Api.Shared;
using Demo.Kodez.Customers.Identity.Api.Shared.Constants;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;

namespace Demo.Kodez.Customers.Identity.Api.Features.NewCustomerPromotion
{
    public interface INewCustomerPromotionService
    {
        Task<Result> AddAsync(CreateCustomerRequest request);
    }
    
    public class NewCustomerPromotionService : INewCustomerPromotionService
    {
        private readonly IValidator<CreateCustomerRequest> _validator;
        private readonly IFeatureManager _featureManager;
        private readonly ICommandHandler<NewCustomerPromotionCommand> _commandHandler;
        private readonly ILogger<NewCustomerPromotionService> _logger;

        public NewCustomerPromotionService(IValidator<CreateCustomerRequest> validator, IFeatureManager featureManager, 
            ICommandHandler<NewCustomerPromotionCommand> commandHandler, ILogger<NewCustomerPromotionService> logger)
        {
            _validator = validator;
            _featureManager = featureManager;
            _commandHandler = commandHandler;
            _logger = logger;
        }

        public async Task<Result> AddAsync(CreateCustomerRequest request)
        {
            var isEnabled = await _featureManager.IsEnabledAsync(Shared.Constants.Features.NewUserPromotion);
            if (!isEnabled)
            {
                _logger.LogInformation("Feature is disabled");
                return Result.Success();
            }
            
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return Result.Failure(ErrorCodes.InvalidRequest, ErrorMessages.InvalidRequest);
            }

            var command = new NewCustomerPromotionCommand
            {
                CustomerId = request.CustomerId,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName
            };
            
            SetPropertiesToAdd();

            var operation = await _commandHandler.ExecuteAsync(command);

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