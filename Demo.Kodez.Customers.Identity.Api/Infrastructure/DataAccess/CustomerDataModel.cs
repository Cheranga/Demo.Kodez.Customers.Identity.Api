using System;
using Azure;
using Azure.Data.Tables;

namespace Demo.Kodez.Customers.Identity.Api.Infrastructure.DataAccess
{
    public class CustomerDataModel : ITableEntity
    {
        public string CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}