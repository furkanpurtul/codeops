using System.Runtime.CompilerServices;

namespace CodeOps.Domain.Abstractions.Violations
{
    public sealed class EnsureContext<TSource>
    {
        private readonly List<DomainViolation> _violations = [];

        internal EnsureContext(DomainViolationSource source)
        {
            ArgumentNullException.ThrowIfNull(source);
            Source = source;
        }

        public DomainViolationSource Source { get; }

        public bool HasViolations => _violations.Count > 0;

        internal void AddViolation(DomainViolation violation)
        {
            ArgumentNullException.ThrowIfNull(violation);
            _violations.Add(violation);
        }

        public GuardContext<TSource, TValue> Guard<TValue>(TValue value, [CallerArgumentExpression(nameof(value))] string memberName = "")
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(memberName);

            return new GuardContext<TSource, TValue>(this, value, memberName);
        }

        public EnsureContext<TSource> Guard<TValue>(TValue value, Action<GuardContext<TSource, TValue>> configure, [CallerArgumentExpression(nameof(value))] string memberName = "")
        {
            ArgumentNullException.ThrowIfNull(configure);

            var guardContext = Guard(value, memberName);
            configure(guardContext);

            return this;
        }

        public EnsureContext<TSource> Check(IRule rule)
        {
            ArgumentNullException.ThrowIfNull(rule);

            if (rule.IsViolated())
            {
                var violation = new DomainViolation
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
                var violation = new DomainViolation
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
            {
                Check(rule);
            }

            return this;
        }

        public EnsureContext<TSource> Check<TContext>(TContext context, params IRule<TContext>[] rules)
        {
            ArgumentNullException.ThrowIfNull(rules);

            foreach (var rule in rules)
            {
                Check(context, rule);
            }

            return this;
        }

        public EnsureContext<TSource> Check<TContext>(TContext context, IEnumerable<IRule<TContext>> rules)
        {
            ArgumentNullException.ThrowIfNull(rules);

            foreach (var rule in rules)
            {
                Check(context, rule);
            }

            return this;
        }

        public EnsureContext<TSource> CheckWhen<TContext>(bool condition, params IRule[] rules)
        {
            return !condition ? this : Check(rules);
        }

        public EnsureContext<TSource> CheckWhen<TContext>(bool condition, IEnumerable<IRule> rules)
        {
            return !condition ? this : Check(rules);
        }

        public EnsureContext<TSource> CheckWhen<TContext>(bool condition, TContext context, params IRule<TContext>[] rules)
        {
            return !condition ? this : Check(context, rules);
        }

        public EnsureContext<TSource> CheckWhen<TContext>(bool condition, TContext context, IEnumerable<IRule<TContext>> rules)
        {
            return !condition ? this : Check(context, rules);
        }

        public IReadOnlyCollection<DomainViolation> GetViolations()
        {
            return _violations.AsReadOnly();
        }

        public EnsureContext<TSource> OrThrow()
        {
            return HasViolations ? throw new DomainViolationException(Source, GetViolations()) : this;
        }
    }
}