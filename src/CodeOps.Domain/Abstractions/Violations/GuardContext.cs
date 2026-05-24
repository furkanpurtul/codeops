using System.Runtime.CompilerServices;

namespace CodeOps.Domain.Abstractions.Violations
{
    public sealed class GuardContext<TSource, TValue>
    {
        private readonly EnsureContext<TSource> _root;

        internal GuardContext(EnsureContext<TSource> root, TValue value, string memberName)
        {
            ArgumentNullException.ThrowIfNull(root);

            _root = root;
            Value = value;
            MemberName = memberName;
        }

        public TValue Value { get; }

        public string MemberName { get; }

        public GuardContext<TSource, TNextValue> Guard<TNextValue>(TNextValue value, [CallerArgumentExpression(nameof(value))] string memberName = "")
        {
            return _root.Guard(value, memberName);
        }

        public GuardContext<TSource, TValue> Check<TContext>(TContext context, params IRule<TContext>[] rules)
        {
            _root.Check(context, rules);
            return this;
        }

        public GuardContext<TSource, TValue> CheckWhen<TContext>(bool condition, TContext context, params IRule<TContext>[] rules)
        {
            _root.CheckWhen(condition, context, rules);
            return this;
        }

        public EnsureContext<TSource> OrThrow()
        {
            return _root.OrThrow();
        }

        internal GuardContext<TSource, TValue> AddViolation(string name, string message)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentException.ThrowIfNullOrWhiteSpace(message);

            var violation = new DomainViolation
            (
                _root.Source,
                ViolationKind.Validation,
                name,
                message,
                MemberName
            );

            _root.AddViolation(violation);

            return this;
        }
    }
}