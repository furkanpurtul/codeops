namespace CodeOps.Domain.Abstractions
{
    /// <summary>
    /// Base type that wires domain invariants (rules) to a context using the CRTP pattern, providing
    /// evaluation and validation helpers.
    /// </summary>
    /// <typeparam name="TContext">
    /// The concrete context type that derives from <see cref="Validatable{TContext}"/>.
    /// </typeparam>
    /// <remarks>
    /// When constructed, if any invariants are provided, they are validated immediately via <see cref="RuleEngine"/>.
    /// Use <see cref="EvaluateInvariants"/> to inspect results without throwing and <see cref="ValidateInvariants"/> to enforce them.
    /// </remarks>
    public abstract class Validatable<TContext> where TContext : Validatable<TContext>
    {
        private readonly IReadOnlyCollection<IRule<TContext>> _invariants;

        /// <summary>
        /// Initializes the validatable context with a set of invariants.
        /// </summary>
        /// <param name="invariants">The rules to enforce; may be empty or <c>null</c>.</param>
        /// <remarks>
        /// If <paramref name="invariants"/> is non-empty, validation is performed immediately and may throw
        /// <see cref="RuleViolationException{TContext}"/> if any rule is violated.
        /// </remarks>
        protected Validatable(IRule<TContext>[] invariants)
        {
            _invariants = invariants ?? [];

            if (_invariants.Count > 0)
                RuleEngine.Validate((TContext)this, _invariants);
        }

        /// <summary>
        /// Evaluates the configured invariants against this context and returns the aggregated result.
        /// </summary>
        /// <returns>A <see cref="RuleEvaluationResult{TContext}"/> with all evaluations.</returns>
        public RuleEvaluationResult<TContext> EvaluateInvariants()
            => RuleEngine.Evaluate((TContext)this, _invariants);

        /// <summary>
        /// Validates the configured invariants against this context, throwing if any are violated.
        /// </summary>
        /// <exception cref="RuleViolationException{TContext}">
        /// Thrown when one or more configured invariants are violated.
        /// </exception>
        public void ValidateInvariants() => RuleEngine.Validate((TContext)this, _invariants);
    }
}
