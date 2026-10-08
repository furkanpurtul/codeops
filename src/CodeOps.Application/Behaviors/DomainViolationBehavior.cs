using CodeOps.Application.Abstractions.Messaging;
using CodeOps.Application.Abstractions.Results;
using CodeOps.Domain.Abstractions.Violations;

namespace CodeOps.Application.Behaviors
{
    public sealed class DomainViolationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : class
    {
        public async Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try
            {
                return await next();
            }
            catch (ViolationException exception)
            {
                var error = DomainViolationErrorMapper.Map(exception);
                return CreateFailure(error);
            }
        }

        private static TResponse CreateFailure(Error error)
        {
            if (typeof(TResponse) == typeof(Result))
                return (TResponse)(object)Result.Failure(error);

            if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
            {
                var valueType = typeof(TResponse).GetGenericArguments()[0];
                var method = typeof(Result)
                    .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                    .First(m => m.Name == nameof(Result.Failure) && m.IsGenericMethodDefinition)
                    .MakeGenericMethod(valueType);

                return (TResponse)method.Invoke(null, [error])!;
            }

            throw new InvalidOperationException($"Domain violations can only be mapped to Result responses. Response type: {typeof(TResponse).Name}.");
        }
    }
}

