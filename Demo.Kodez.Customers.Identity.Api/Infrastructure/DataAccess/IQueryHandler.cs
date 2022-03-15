using System.Collections.Generic;
using System.Threading.Tasks;
using Demo.Kodez.Customers.Identity.Api.Shared;

namespace Demo.Kodez.Customers.Identity.Api.Infrastructure.DataAccess
{
    public interface IQuery
    {
        
    }
    
    public interface IQueryHandler<TQuery, TResponse> where TQuery:IQuery where TResponse:class
    {
        Task<Result<List<TResponse>>> GetAsync(TQuery query);
    }
}