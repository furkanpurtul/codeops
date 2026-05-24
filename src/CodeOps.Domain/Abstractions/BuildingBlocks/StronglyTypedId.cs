namespace CodeOps.Domain.Abstractions.BuildingBlocks
{
    public abstract record StronglyTypedId<TValue> : IStronglyTypedId
        where TValue : notnull
    {
        public TValue Value { get; }

        protected StronglyTypedId(TValue value)
        {
            Value = value;
        }

        public bool IsDefault() => EqualityComparer<TValue>.Default.Equals(Value, default);

        public override string ToString() => Value.ToString() ?? string.Empty;

        public static implicit operator TValue(StronglyTypedId<TValue> id) => id.Value;
    }
}
