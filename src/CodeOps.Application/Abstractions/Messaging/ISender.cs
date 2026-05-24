namespace CodeOps.Application.Abstractions.Messaging
{
    public interface ISender
    {
        Task SendAsync(ICommand command, CancellationToken cancellationToken = default);

        Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);

        Task<TResponse> QueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
    }
}