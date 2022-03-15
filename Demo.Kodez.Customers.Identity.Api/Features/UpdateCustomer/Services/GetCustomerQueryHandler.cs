using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Demo.Kodez.Customers.Identity.Api.Extensions;
using Demo.Kodez.Customers.Identity.Api.Infrastructure.DataAccess;
using Demo.Kodez.Customers.Identity.Api.Shared;
using Demo.Kodez.Customers.Identity.Api.Shared.Constants;
using Microsoft.Extensions.Logging;

namespace Demo.Kodez.Customers.Identity.Api.Features.UpdateCustomer.Services
{
    public class GetCustomerQueryHandler : IQueryHandler<GetCustomerQuery, CustomerDataModel>
    {
        private const string TableName = "customers";
        private readonly ILogger<GetCustomerQueryHandler> _logger;
        private readonly TableServiceClient _serviceClient;

        public GetCustomerQueryHandler(TableServiceClient serviceClient, ILogger<GetCustomerQueryHandler> logger)
        {
            _serviceClient = serviceClient;
            _logger = logger;
        }

        public async Task<Result<List<CustomerDataModel>>> GetAsync(GetCustomerQuery query)
        {
            try
            {
                var tableClient = _serviceClient.GetTableClient(TableName);

                var results = tableClient.QueryAsync<CustomerDataModel>(model => model.PartitionKey == "ACTIVE" && model.RowKey == query.CustomerId.ToUpper());
                var list = await results.AsPages().ToListAsync();

                var customers = list.SelectMany(x => x.Values).ToList();

                return Result<List<CustomerDataModel>>.Success(customers);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, ErrorMessages.GetCustomerDataError);
            }

            return Result<List<CustomerDataModel>>.Failure(ErrorCodes.GetCustomerDataError, ErrorMessages.GetCustomerDataError);
        }
    }
}