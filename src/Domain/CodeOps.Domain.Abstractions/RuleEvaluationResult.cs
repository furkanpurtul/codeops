namespace CodeOps.Domain.Abstractions
{
    /// <summary>
    /// Aggregates the outcomes of one or more rule evaluations for a given context.
    /// </summary>
    /// <typeparam name="TContext">The context type the rules evaluate.</typeparam>
    /// <remarks>
    /// Provides convenience access to all evaluations, the subset of violated ones,
    /// and an overall validity flag. Use <see cref="Valid"/> for an empty, valid result.
    /// </remarks>
    public sealed class RuleEvaluationResult<TContext>
    {
        /// <summary>
        /// Gets the evaluations performed against the context.
        /// </summary>
        public IReadOnlyList<RuleEvaluation<TContext>> Evaluations { get; }

        /// <summary>
        /// Gets a value indicating whether all evaluated rules are valid (no violations).
        /// </summary>
        public bool IsValid { get; }

        /// <summary>
        /// Gets the evaluations that resulted in rule violations.
        /// </summary>
        public IEnumerable<RuleEvaluation<TContext>> Violated { get; }

        /// <summary>
        /// Initializes a new result for a single rule evaluation.
        /// </summary>
        /// <param name="evaluation">The rule evaluation.</param>
        internal RuleEvaluationResult(RuleEvaluation<TContext> evaluation)
        {
            Evaluations = [evaluation];
            var violated = Evaluations.Where(e => e.IsViolated).ToArray();
            Violated = violated;
            IsValid = violated.Length == 0;
        }

        /// <summary>
        /// Initializes a new result for a collection of rule evaluations.
        /// </summary>
        /// <param name="evaluations">The rule evaluations.</param>
        internal RuleEvaluationResult(IReadOnlyList<RuleEvaluation<TContext>> evaluations)
        {
            Evaluations = evaluations;
            var violated = evaluations.Where(e => e.IsViolated).ToArray();
            Violated = violated;
            IsValid = violated.Length == 0;
        }

        /// <summary>
        /// Gets a reusable valid result with no evaluations.
        /// </summary>
        public static RuleEvaluationResult<TContext> Valid { get; } = new([]);

        /// <summary>
        /// Creates a result from a single evaluation, returning <see cref="Valid"/> when not violated.
        /// </summary>
        /// <param name="evaluation">The rule evaluation.</param>
        internal static RuleEvaluationResult<TContext> From(RuleEvaluation<TContext> evaluation)
            => evaluation.IsViolated ? new RuleEvaluationResult<TContext>(evaluation) : Valid;

        /// <summary>
        /// Creates a result from multiple evaluations, returning <see cref="Valid"/> when the list is empty.
        /// </summary>
        /// <param name="evaluations">The rule evaluations.</param>
        internal static RuleEvaluationResult<TContext> From(IReadOnlyList<RuleEvaluation<TContext>> evaluations)
            => evaluations.Count == 0 ? Valid : new RuleEvaluationResult<TContext>(evaluations);
    }
}
