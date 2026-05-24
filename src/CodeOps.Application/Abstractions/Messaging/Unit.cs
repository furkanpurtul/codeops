namespace CodeOps.Application.Abstractions.Messaging
{
    public readonly record struct Unit
    {
        public static readonly Unit Value = new();
    }
}