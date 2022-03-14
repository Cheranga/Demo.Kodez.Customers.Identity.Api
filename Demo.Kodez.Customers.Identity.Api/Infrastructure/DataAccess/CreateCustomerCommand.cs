namespace Demo.Kodez.Customers.Identity.Api.Infrastructure.DataAccess
{
    public class CreateCustomerCommand : ICommand
    {
        public string CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}