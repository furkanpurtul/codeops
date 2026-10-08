using CodeOps.Application.Abstractions.Results;
using CodeOps.Application.Abstractions.Messaging;
using CodeOps.Application.Abstractions.Validation;
using CodeOps.Domain.Abstractions.Violations;

namespace CodeOps.Application.Behaviors
{
    public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : class
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var validator in _validators)
                    validator.Validate(request);
            }
            catch (ViolationException exception)
            {
                return CreateFailure(DomainViolationErrorMapper.Map(exception));
            }

            return await next();
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

            throw new InvalidOperationException($"Validation behavior can only map failures to Result responses. Response type: {typeof(TResponse).Name}.");
        }
    }
}
