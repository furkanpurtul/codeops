namespace CodeOps.Domain.Abstractions
{
    /// <summary>
    /// Contract for a domain rule/invariant that can describe itself and evaluate violation against a context.
    /// </summary>
    /// <typeparam name="TContext">The context type the rule evaluates.</typeparam>
    public interface IRule<TContext>
    {
        /// <summary>
        /// Provides a human-readable description of the rule based on the given <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The context to describe in relation to the rule.</param>
        /// <returns>A descriptive string intended for diagnostics or validation messages.</returns>
        string Describe(TContext context);

        /// <summary>
        /// Determines whether the rule is violated by the given <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The context to evaluate.</param>
        /// <returns><c>true</c> if the rule is violated; otherwise, <c>false</c>.</returns>
        bool IsViolatedBy(TContext context);
    }
}