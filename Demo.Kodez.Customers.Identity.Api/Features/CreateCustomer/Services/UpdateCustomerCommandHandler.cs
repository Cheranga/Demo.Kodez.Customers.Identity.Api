using System.Threading.Tasks;
using Azure.Data.Tables;
using Demo.Kodez.Customers.Identity.Api.Infrastructure.DataAccess;
using Demo.Kodez.Customers.Identity.Api.Shared;
using Demo.Kodez.Customers.Identity.Api.Shared.Constants;
using Microsoft.Extensions.Logging;

namespace Demo.Kodez.Customers.Identity.Api.Features.CreateCustomer.Services
{
    public class UpdateCustomerCommandHandler : ICommandHandler<UpdateCustomerCommand>
    {
        private readonly TableServiceClient _serviceClient;
        private readonly ILogger<UpdateCustomerCommandHandler> _logger;
        private const string CustomersTable = "customers";

        public UpdateCustomerCommandHandler(TableServiceClient serviceClient, ILogger<UpdateCustomerCommandHandler> logger)
        {
            _serviceClient = serviceClient;
            _logger = logger;
        }
        public async Task<Result> ExecuteAsync(UpdateCustomerCommand command)
        {
            var tableClient = _serviceClient.GetTableClient(CustomersTable);
            if (tableClient == null)
            {
                return Result.Failure(ErrorCodes.TableClientNotFound, ErrorMessages.TableClientNotFound);
            }

            var entity = GetTableEntity(command);
            var operation = await SaveAsync(tableClient,entity);
            
            return operation;
        }

        private TableEntity GetTableEntity(UpdateCustomerCommand command)
        {
            var partitionKey = "ACTIVE";
            var rowKey = command.CustomerId.ToUpper();

            var entity = new TableEntity(partitionKey, rowKey);

            if (!string.IsNullOrEmpty(command.Email))
            {
                entity.Add(nameof(command.Email), command.Email);
            }

            if (!string.IsNullOrEmpty(command.FirstName))
            {
                entity.Add(nameof(command.FirstName), command.FirstName);
            }
            
            if (!string.IsNullOrEmpty(command.LastName))
            {
                entity.Add(nameof(command.LastName), command.LastName);
            }

            return entity;
        }

        private async Task<Result> SaveAsync(TableClient client, TableEntity entity)
        {
            var response = await client.UpsertEntityAsync(entity);
            if (response.IsError)
            {
                _logger.LogError("cannot create customer: {FailedReason}", response.ReasonPhrase);
                return Result.Failure(ErrorCodes.UpdateError, ErrorMessages.UpdateError);
            }

            return Result.Success();
        }
    }
}