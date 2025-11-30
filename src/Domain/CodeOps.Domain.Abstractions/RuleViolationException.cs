namespace CodeOps.Domain.Abstractions
{
    /// <summary>
    /// Exception thrown when one or more domain rules are violated for a given context.
    /// </summary>
    /// <typeparam name="TContext">The context type the rules evaluate.</typeparam>
    /// <remarks>
    /// Carries the offending <see cref="Context"/> and the aggregated <see cref="Result"/> of evaluations.
    /// The exception message is constructed from rule descriptions of violated evaluations.
    /// </remarks>
    public sealed class RuleViolationException<TContext> : Exception
    {
        /// <summary>
        /// Gets the context instance for which rule violations were detected.
        /// </summary>
        public TContext Context { get; }

        /// <summary>
        /// Gets the aggregated evaluation result containing violated rules.
        /// </summary>
        public RuleEvaluationResult<TContext> Result { get; }

        /// <summary>
        /// Initializes a new instance of the exception with the specified context and evaluation result.
        /// </summary>
        /// <param name="context">The evaluated context.</param>
        /// <param name="result">The rule evaluation result.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/> is null.</exception>
        public RuleViolationException(TContext context, RuleEvaluationResult<TContext> result)
            : base(BuildMessage(context, result))
        {
            Context = context;
            Result = result ?? throw new ArgumentNullException(nameof(result));
        }

        /// <summary>
        /// Builds a human-readable message based on violated rule descriptions.
        /// </summary>
        /// <param name="context">The evaluated context.</param>
        /// <param name="evaluation">The rule evaluation result.</param>
        /// <returns>A message summarizing the violations, or a no-violations note.</returns>
        private static string BuildMessage(TContext context, RuleEvaluationResult<TContext> evaluation)
        {
            var violatedEvaluations = evaluation.Violated
                .Select(e => e.Rule.Describe(context))
                .ToArray();

            if (violatedEvaluations.Length == 0)
                return $"No rule violations for {typeof(TContext).Name}.";

            var prefix = $"Rule violations for {typeof(TContext).Name}";

            var message = $"{prefix}: {string.Join(", ", violatedEvaluations)}";
            return message;
        }
    }
}
