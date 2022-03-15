using System.Linq;
using System.Threading.Tasks;
using Demo.Kodez.Customers.Identity.Api.Features.UpdateCustomer.Models;
using Demo.Kodez.Customers.Identity.Api.Infrastructure.DataAccess;
using Demo.Kodez.Customers.Identity.Api.Shared;
using Demo.Kodez.Customers.Identity.Api.Shared.Constants;
using FluentValidation;
using Microsoft.FeatureManagement;

namespace Demo.Kodez.Customers.Identity.Api.Features.UpdateCustomer.Services
{
    public interface IUpdateCustomerService
    {
        Task<Result> UpdateAsync(UpdateCustomerRequest request);
    }

    public class UpdateCustomerService : IUpdateCustomerService
    {
        private readonly IFeatureManager _featureManager;
        private readonly IValidator<UpdateCustomerRequest> _validator;
        private readonly IQueryHandler<GetCustomerQuery, CustomerDataModel> _queryHandler;
        private readonly ICommandHandler<UpdateCustomerCommand> _commandHandler;

        public UpdateCustomerService(IFeatureManager featureManager, IValidator<UpdateCustomerRequest> validator,
            IQueryHandler<GetCustomerQuery, CustomerDataModel> queryHandler,
            ICommandHandler<UpdateCustomerCommand> commandHandler)
        {
            _featureManager = featureManager;
            _validator = validator;
            _queryHandler = queryHandler;
            _commandHandler = commandHandler;
        }

        public async Task<Result> UpdateAsync(UpdateCustomerRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Result.Failure(ErrorCodes.InvalidRequest, validationResult);
            }

            var getCustomersOperation = await _queryHandler.GetAsync(new GetCustomerQuery
            {
                CustomerId = request.CustomerId
            });

            if (!getCustomersOperation.Status)
            {
                return Result.Failure(getCustomersOperation.ErrorCode, getCustomersOperation.ValidationResult);
            }

            var customers = getCustomersOperation.Data;
            if (customers == null || !customers.Any())
            {
                return Result.Failure(ErrorCodes.CustomerDoesNotExist, ErrorMessages.CustomerDoesNotExist);
            }

            var command = new UpdateCustomerCommand
            {
                CustomerId = request.CustomerId,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName
            };
            
            await SetPropertiesToUpdateAsync();
            var operation = await _commandHandler.ExecuteAsync(command);

            return operation;
        }

        private async Task SetPropertiesToUpdateAsync()
        {
            var canUpdateEmail = await _featureManager.IsEnabledAsync("UpdateEmail");

            _commandHandler
                .Add(x => x.CustomerId)
                .Add(x => x.FirstName)
                .Add(x => x.LastName);

            if (canUpdateEmail)
            {
                _commandHandler.Add(x => x.Email);
            }
        }
    }
}