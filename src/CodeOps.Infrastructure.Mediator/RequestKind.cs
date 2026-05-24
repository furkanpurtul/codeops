namespace CodeOps.Infrastructure.Mediator
{
    internal enum RequestKind
    {
        CommandWithoutResponse = 1,
        CommandWithResponse = 2,
        Query = 3
    }
}
