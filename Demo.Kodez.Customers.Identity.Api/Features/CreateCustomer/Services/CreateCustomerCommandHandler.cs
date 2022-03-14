using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Demo.Kodez.Customers.Identity.Api.Infrastructure.DataAccess;
using Demo.Kodez.Customers.Identity.Api.Shared;
using Demo.Kodez.Customers.Identity.Api.Shared.Constants;
using Microsoft.Extensions.Logging;

namespace Demo.Kodez.Customers.Identity.Api.Features.CreateCustomer.Services
{
    public class CreateCustomerCommandHandler : ICommandHandler<CreateCustomerCommand>
    {
        private const string CustomersTable = "customers";
        
        private readonly TableServiceClient _serviceClient;
        private readonly ILogger<CreateCustomerCommandHandler> _logger;

        public CreateCustomerCommandHandler(TableServiceClient serviceClient, ILogger<CreateCustomerCommandHandler> logger)
        {
            _serviceClient = serviceClient;
            _logger = logger;
        }
        
        public async Task<Result> ExecuteAsync(CreateCustomerCommand command)
        {
            var tableClient = _serviceClient.GetTableClient(CustomersTable);
            if (tableClient == null)
            {
                return Result.Failure(ErrorCodes.TableClientNotFound, ErrorMessages.TableClientNotFound);
            }
            
            var partitionKey = "ACTIVE";
            var rowKey = command.CustomerId.ToUpper();
            var entity = new TableEntity(partitionKey, rowKey)
            {
                {nameof(command.FirstName), command.FirstName},
                {nameof(command.LastName), command.LastName},
                {nameof(command.Email), command.Email}
            };

            var response = await tableClient.UpsertEntityAsync(entity);
            if (response.IsError)
            {
                _logger.LogError("cannot create customer: {FailedReason}", response.ReasonPhrase);
                return Result.Failure(ErrorCodes.InsertError, ErrorMessages.InsertError);
            }

            return Result.Success();
        }
    }
}