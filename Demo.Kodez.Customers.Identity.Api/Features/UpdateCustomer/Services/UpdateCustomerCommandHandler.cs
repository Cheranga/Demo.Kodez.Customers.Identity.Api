using System;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Demo.Kodez.Customers.Identity.Api.Infrastructure.DataAccess;
using Demo.Kodez.Customers.Identity.Api.Shared;
using Demo.Kodez.Customers.Identity.Api.Shared.Constants;
using Microsoft.Extensions.Logging;

namespace Demo.Kodez.Customers.Identity.Api.Features.UpdateCustomer.Services
{
    public class UpdateCustomerCommandHandler : CommandHandlerBase<UpdateCustomerCommand>
    {
        public UpdateCustomerCommandHandler(TableServiceClient serviceClient, ILogger<UpdateCustomerCommandHandler> logger) : base(serviceClient, logger)
        {
        }

        protected override string TableName => "customers";
        protected override string ErrorCode => ErrorCodes.UpdateError;
        protected override string ErrorMessage => ErrorMessages.UpdateError;

        protected override TableEntity GetTableEntity(UpdateCustomerCommand command)
        {
            var entity = new TableEntity("ACTIVE", command.CustomerId);
            return entity;
        }

        protected override async Task<Result> SaveAsync(TableClient client, TableEntity entity)
        {
            try
            {
                var response = await client.UpsertEntityAsync(entity);
                if (response.IsError)
                {
                    Logger.LogError("cannot update customer: {FailedReason}", response.ReasonPhrase);
                    return Result.Failure(ErrorCode, ErrorMessage);
                }

                return Result.Success();
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, ErrorMessage);
            }

            return Result.Failure(ErrorCode, ErrorMessage);
        }
    }
}