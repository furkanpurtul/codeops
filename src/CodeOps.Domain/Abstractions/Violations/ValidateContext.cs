namespace CodeOps.Domain.Abstractions.Violations
{
    public sealed class ValidateContext<TSource, TValue> : IConstraintContext<TSource, TValue>
    {
        private readonly EnsureContext<TSource> _ensureContext;

        public TValue Value { get; }

        public string MemberName { get; }

        internal ValidateContext(EnsureContext<TSource> ensureContext, TValue value, string memberName)
        {
            ArgumentNullException.ThrowIfNull(ensureContext);
            ArgumentException.ThrowIfNullOrWhiteSpace(memberName);

            _ensureContext = ensureContext;
            Value = value;
            MemberName = memberName;
        }

        public void AddViolation(string message)
        {
            _ensureContext.AddViolation
            (
                ViolationKind.Validation,
                MemberName,
                message
            );
        }

        public EnsureContext<TSource> Done()
        {
            return _ensureContext;
        }
    }
}
