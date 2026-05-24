using CodeOps.Application.Abstractions.Messaging;
using CodeOps.Application.Abstractions.Results;
using CodeOps.Domain.Abstractions.Violations;

namespace CodeOps.Application.Behaviors
{
    //public sealed class DomainViolationBehavior<TRequest> : IPipelineBehavior<TRequest, Result>
    //    where TRequest : class
    //{
    //    public async Task<Result> HandleAsync(TRequest request, RequestHandlerDelegate<Result> next, CancellationToken cancellationToken)
    //    {
    //        try
    //        {
    //            return await next();
    //        }
    //        catch (DomainViolationException exception)
    //        {
    //            var error = DomainViolationErrorMapper.Map(exception);
    //            return Result.Failure(error);
    //        }
    //    }
    //}

    public sealed class DomainViolationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse>>
        where TRequest : class
    {
        public async Task<Result<TResponse>> HandleAsync(TRequest request, RequestHandlerDelegate<Result<TResponse>> next, CancellationToken cancellationToken)
        {
            try
            {
                return await next();
            }
            catch (DomainViolationException exception)
            {
                var error = DomainViolationErrorMapper.Map(exception);
                var result = Result<TResponse>.Failure(error);

                return result;
            }
        }
    }
}
