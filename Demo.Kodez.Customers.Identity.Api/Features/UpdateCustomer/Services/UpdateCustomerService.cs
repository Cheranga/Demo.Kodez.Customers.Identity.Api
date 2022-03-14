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
        private readonly ICommandHandler<UpdateCustomerCommand> _commandHandler;

        public UpdateCustomerService(IFeatureManager featureManager, IValidator<UpdateCustomerRequest> validator, ICommandHandler<UpdateCustomerCommand> commandHandler)
        {
            _featureManager = featureManager;
            _validator = validator;
            _commandHandler = commandHandler;
        }

        public async Task<Result> UpdateAsync(UpdateCustomerRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Result.Failure(ErrorCodes.InvalidRequest, validationResult);
            }

            var command = await GetCommand(request);
            var operation = await _commandHandler.ExecuteAsync(command);
            
            return operation;
        }

        private async Task<UpdateCustomerCommand> GetCommand(UpdateCustomerRequest request)
        {
            var canUpdateEmail = await _featureManager.IsEnabledAsync("UpdateEmail");

            var command = new UpdateCustomerCommand
            {
                CustomerId = request.CustomerId,
                Email = canUpdateEmail? request.Email : null,
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            return command;
        }
    }
}