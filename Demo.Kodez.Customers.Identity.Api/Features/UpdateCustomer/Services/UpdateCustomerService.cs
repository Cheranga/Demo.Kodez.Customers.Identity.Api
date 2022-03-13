using System.Threading.Tasks;
using Demo.Kodez.Customers.Identity.Api.Features.UpdateCustomer.Models;
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

        public UpdateCustomerService(IFeatureManager featureManager, IValidator<UpdateCustomerRequest> validator)
        {
            _featureManager = featureManager;
            _validator = validator;
        }
        
        public async Task<Result> UpdateAsync(UpdateCustomerRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Result.Failure(ErrorCodes.InvalidRequest, validationResult);
            }
            
            var canUpdateEmail = await _featureManager.IsEnabledAsync(Shared.Constants.Features.UpdateEmail);

            return Result.Success();

        }
    }
}