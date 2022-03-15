using Demo.Kodez.Customers.Identity.Api.Infrastructure.DataAccess;

namespace Demo.Kodez.Customers.Identity.Api.Features.UpdateCustomer.Services
{
    public class GetCustomerQuery : IQuery
    {
        public string CustomerId { get; set; }
    }
}