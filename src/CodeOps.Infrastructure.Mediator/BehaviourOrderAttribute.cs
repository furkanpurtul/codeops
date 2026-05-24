namespace CodeOps.Infrastructure.Mediator
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class BehaviorOrderAttribute : Attribute
    {
        public BehaviorOrderAttribute(int order)
        {
            Order = order;
        }

        public int Order { get; }
    }
}
