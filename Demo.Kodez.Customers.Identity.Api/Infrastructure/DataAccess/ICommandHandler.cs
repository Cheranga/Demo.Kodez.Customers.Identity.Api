using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Demo.Kodez.Customers.Identity.Api.Shared;

namespace Demo.Kodez.Customers.Identity.Api.Infrastructure.DataAccess
{
    public interface ICommand
    {
    }

    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        Task<Result> ExecuteAsync(TCommand command);

        ICommandHandler<TCommand> Add(Expression<Func<TCommand, object>> propertyFunc);
    }
}