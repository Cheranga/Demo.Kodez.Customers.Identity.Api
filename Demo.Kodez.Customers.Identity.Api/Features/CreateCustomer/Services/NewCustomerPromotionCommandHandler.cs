using System;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Demo.Kodez.Customers.Identity.Api.Infrastructure.DataAccess;
using Demo.Kodez.Customers.Identity.Api.Shared;
using Demo.Kodez.Customers.Identity.Api.Shared.Constants;
using Microsoft.Extensions.Logging;

namespace Demo.Kodez.Customers.Identity.Api.Features.CreateCustomer.Services
{
    public class NewCustomerPromotionCommandHandler : CommandHandlerBase<NewCustomerPromotionCommand>
    {
        public NewCustomerPromotionCommandHandler(TableServiceClient serviceClient, ILogger<NewCustomerPromotionCommandHandler> logger) : base(serviceClient, logger)
        {
        }

        protected override string TableName => "newcustomerpromotions";
        protected override string ErrorCode => ErrorCodes.InsertError;
        protected override string ErrorMessage => ErrorMessages.InsertError;
        protected override TableEntity GetTableEntity(NewCustomerPromotionCommand command)
        {
            return new TableEntity("NEW", command.CustomerId.ToUpper());
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