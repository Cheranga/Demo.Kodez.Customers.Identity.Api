namespace Demo.Kodez.Customers.Identity.Api.Features.CreateCustomer.Models
{
    public class CreateCustomerRequest
    {
        public string CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}