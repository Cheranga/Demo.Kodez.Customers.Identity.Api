using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Demo.Kodez.Customers.Identity.Api.Shared;
using Microsoft.Extensions.Logging;

namespace Demo.Kodez.Customers.Identity.Api.Infrastructure.DataAccess
{
    public abstract class CommandHandlerBase<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        private readonly TableServiceClient _serviceClient;
        protected readonly ILogger<CommandHandlerBase<TCommand>> Logger;
        private List<Expression<Func<TCommand, object>>> propertyFuncs = new();

        protected abstract string TableName { get; }
        protected abstract string ErrorCode { get; }
        protected abstract string ErrorMessage { get; }

        protected abstract TableEntity GetTableEntity(TCommand command);

        protected abstract Task<Result> SaveAsync(TableClient client, TableEntity entity);

        protected CommandHandlerBase(TableServiceClient serviceClient, ILogger<CommandHandlerBase<TCommand>> logger)
        {
            _serviceClient = serviceClient;
            Logger = logger;
        }
        
        public virtual async Task<Result> ExecuteAsync(TCommand command)
        {
            var tableClient = _serviceClient.GetTableClient(TableName);
            if (tableClient == null)
            {
                return Result.Failure(ErrorCode,ErrorMessage);
            }

            var entity = GetTableEntity(command);
            SetupProperties(entity, command);
            var operation = await SaveAsync(tableClient,entity);
            
            return operation;
        }
        
        protected virtual void SetupProperties(TableEntity entity, TCommand command)
        {
            foreach (var propertyFunc in propertyFuncs)
            {
                var propertyName = (propertyFunc.Body as MemberExpression)?.Member.Name;
                if (string.IsNullOrEmpty(propertyName))
                {
                    continue;
                }
                entity.Add(propertyName, propertyFunc.Compile()(command));
            }
        }
        
        public ICommandHandler<TCommand> Add(Expression<Func<TCommand, object>> propertyFunc)
        {
            propertyFuncs.Add(propertyFunc);
            return this;
        }
    }
}