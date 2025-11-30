namespace CodeOps.Domain.Abstractions
{
    /// <summary>
    /// Represents the outcome of evaluating a single <see cref="IRule{TContext}"/> against a context.
    /// </summary>
    /// <typeparam name="TContext">The context type the rule evaluates.</typeparam>
    /// <param name="Rule">The evaluated rule.</param>
    /// <param name="IsViolated">Indicates whether the rule was violated for the given context.</param>
    /// <remarks>
    /// Used by <see cref="RuleEngine"/> to aggregate results into <see cref="RuleEvaluationResult{TContext}"/>.
    /// </remarks>
    public sealed record RuleEvaluation<TContext>(IRule<TContext> Rule, bool IsViolated);
}
