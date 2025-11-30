namespace CodeOps.Domain.Abstractions
{
    /// <summary>
    /// Groups a named set of domain rules/invariants for a given context type.
    /// </summary>
    /// <typeparam name="TContext">The context type the rules evaluate.</typeparam>
    /// <remarks>
    /// Useful for organizing validation policies (e.g., creation vs. update rules) and passing
    /// them to the <see cref="RuleEngine"/> for evaluation or validation.
    /// </remarks>
    public sealed class RuleSet<TContext>
    {
        /// <summary>
        /// Gets the display name of this rule set (e.g., &quot;CreateOrder&quot;, &quot;UpdateCustomer&quot;).
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the rules contained in this set.
        /// </summary>
        public IReadOnlyList<IRule<TContext>> Rules { get; }

        /// <summary>
        /// Initializes a new <see cref="RuleSet{TContext}"/> with the specified name and rules.
        /// </summary>
        /// <param name="name">The display name of the rule set.</param>
        /// <param name="rules">The rules to include in the set.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null.</exception>
        /// <remarks>
        /// When <paramref name="rules"/> is null, the set is initialized as empty.
        /// </remarks>
        public RuleSet(string name, params IRule<TContext>[] rules)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Rules = rules ?? [];
        }
    }
}
