using System;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Demo.Kodez.Customers.Identity.Api.Infrastructure.DataAccess;
using Demo.Kodez.Customers.Identity.Api.Shared;
using Demo.Kodez.Customers.Identity.Api.Shared.Constants;
using Microsoft.Extensions.Logging;

namespace Demo.Kodez.Customers.Identity.Api.Features.CreateCustomer.Services
{
    public class CreateCustomerCommandHandler : CommandHandlerBase<CreateCustomerCommand>
    {
        public CreateCustomerCommandHandler(TableServiceClient serviceClient, ILogger<CreateCustomerCommandHandler> logger) : base(serviceClient, logger)
        {
        }

        protected override string TableName => "customers";
        protected override string ErrorCode => ErrorCodes.InsertError;
        protected override string ErrorMessage => ErrorMessages.InsertError;

        protected override TableEntity GetTableEntity(CreateCustomerCommand command)
        {
            var entity = new TableEntity("ACTIVE", command.CustomerId.ToUpper());
            return entity;
        }

        protected override async Task<Result> SaveAsync(TableClient client, TableEntity entity)
        {
            try
            {
                var response = await client.AddEntityAsync(entity);
                if (response.IsError)
                {
                    Logger.LogError("cannot create customer: {FailedReason}", response.ReasonPhrase);
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