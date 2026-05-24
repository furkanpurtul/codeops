namespace CodeOps.Application.Abstractions.Messaging
{
    public interface ICommand;

    public interface ICommand<out TResponse>;
}
