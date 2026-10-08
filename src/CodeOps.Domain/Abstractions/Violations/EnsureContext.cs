using System.Runtime.CompilerServices;

namespace CodeOps.Domain.Abstractions.Violations
{
    public sealed class EnsureContext<TSource>
    {
        private readonly List<Violation> _violations = [];

        internal EnsureContext(ViolationSource source)
        {
            ArgumentNullException.ThrowIfNull(source);

            Source = source;
        }

        public ViolationSource Source { get; }

        public bool HasViolations => _violations.Count > 0;

        public GuardContext<TSource, TValue> Guard<TValue>(TValue value, [CallerArgumentExpression(nameof(value))] string memberName = "")
        {
            var context = new GuardContext<TSource, TValue>(this, value, memberName);
            return context;
        }

        public ValidateContext<TSource, TValue> Validate<TValue>(TValue value, [CallerArgumentExpression(nameof(value))] string memberName = "")
        {
            var context = new ValidateContext<TSource, TValue>(this, value, memberName);
            return context;
        }

        public GuardContext<TSource, TValue> Guard<TValue>(TValue value, out TValue output, [CallerArgumentExpression(nameof(value))] string memberName = "")
        {
            var context = new GuardContext<TSource, TValue>(this, value, memberName);
            output = context.Value;
            return context;
        }

        public ValidateContext<TSource, TValue> Validate<TValue>(TValue value, out TValue output, [CallerArgumentExpression(nameof(value))] string memberName = "")
        {
            var context = new ValidateContext<TSource, TValue>(this, value, memberName);
            output = context.Value;
            return context;
        }

        public EnsureContext<TSource> Guard<TValue>
        (
            TValue value, 
            Action<GuardContext<TSource, TValue>> configure, 
            [CallerArgumentExpression(nameof(value))] string memberName = ""
        )
        {
            ArgumentNullException.ThrowIfNull(configure);

            var guardContext = Guard(value, memberName);

            configure(guardContext);

            return this;
        }

        public EnsureContext<TSource> Validate<TValue>
        (
            TValue value,
            Action<ValidateContext<TSource, TValue>> configure,
            [CallerArgumentExpression(nameof(value))] string memberName = ""
        )
        {
            ArgumentNullException.ThrowIfNull(configure);

            var validateContext = Validate(value, memberName);

            configure(validateContext);

            return this;
        }

        public EnsureContext<TSource> Check(IRule rule)
        {
            ArgumentNullException.ThrowIfNull(rule);

            if (rule.IsViolated())
            {
                var violation = new Violation
                (
                    Source,
                    rule.Kind,
                    rule.GetType().Name,
                    rule.Describe()
                );

                AddViolation(violation);
            }

            return this;
        }

        public EnsureContext<TSource> Check<TContext>(TContext context, IRule<TContext> rule)
        {
            ArgumentNullException.ThrowIfNull(rule);

            if (rule.IsViolatedBy(context))
            {
                var violation = new Violation
                (
                    Source,
                    rule.Kind,
                    rule.GetType().Name,
                    rule.Describe(context)
                );

                AddViolation(violation);
            }

            return this;
        }

        public EnsureContext<TSource> Check(params IRule[] rules)
        {
            ArgumentNullException.ThrowIfNull(rules);

            foreach (var rule in rules)
                Check(rule);

            return this;
        }

        public EnsureContext<TSource> Check(IEnumerable<IRule> rules)
        {
            ArgumentNullException.ThrowIfNull(rules);

            foreach (var rule in rules)
                Check(rule);

            return this;
        }

        public EnsureContext<TSource> Check<TContext>(TContext context, params IRule<TContext>[] rules)
        {
            ArgumentNullException.ThrowIfNull(rules);

            foreach (var rule in rules)
                Check(context, rule);

            return this;
        }

        public EnsureContext<TSource> Check<TContext>(TContext context, IEnumerable<IRule<TContext>> rules)
        {
            ArgumentNullException.ThrowIfNull(rules);

            foreach (var rule in rules)
                Check(context, rule);

            return this;
        }

        public EnsureContext<TSource> CheckWhen(bool condition, params IRule[] rules)
        {
            return condition
                ? Check(rules)
                : this;
        }

        public EnsureContext<TSource> CheckWhen(bool condition, IEnumerable<IRule> rules)
        {
            return condition
                ? Check(rules)
                : this;
        }

        public EnsureContext<TSource> CheckWhen<TContext>(bool condition, TContext context, params IRule<TContext>[] rules)
        {
            return condition
                ? Check(context, rules)
                : this;
        }

        public EnsureContext<TSource> CheckWhen<TContext>(bool condition, TContext context, IEnumerable<IRule<TContext>> rules)
        {
            return condition
                ? Check(context, rules)
                : this;
        }

        public IReadOnlyCollection<Violation> GetViolations()
        {
            return _violations.AsReadOnly();
        }

        public EnsureContext<TSource> OrThrow()
        {
            return !HasViolations 
                ? this 
                : throw new ViolationException(Source, GetViolations());
        }

        internal void AddViolation(Violation violation)
        {
            ArgumentNullException.ThrowIfNull(violation);

            _violations.Add(violation);
        }

        internal void AddViolation(ViolationKind kind, string memberName, string message)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(memberName);
            ArgumentException.ThrowIfNullOrWhiteSpace(message);

            var violation = new Violation
            (
                Source,
                kind,
                memberName,
                message
            );

            AddViolation(violation);
        }
    }
}