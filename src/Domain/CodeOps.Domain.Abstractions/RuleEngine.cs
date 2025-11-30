namespace CodeOps.Domain.Abstractions
{
    /// <summary>
    /// Static helper for evaluating and validating domain rules/invariants against a context.
    /// </summary>
    /// <remarks>
    /// Provides single-rule and multi-rule (collection or <see cref="RuleSet{TContext}"/>) evaluation overloads,
    /// returning <see cref="RuleEvaluationResult{TContext}"/> or throwing <see cref="RuleViolationException{TContext}"/> on validation.
    /// </remarks>
    public static class RuleEngine
    {
        /// <summary>
        /// Evaluates a single rule against the given context.
        /// </summary>
        /// <typeparam name="TContext">The context type the rule evaluates.</typeparam>
        /// <param name="context">The context to evaluate.</param>
        /// <param name="rule">The rule to evaluate.</param>
        /// <returns>An evaluation result containing the rule outcome.</returns>
        public static RuleEvaluationResult<TContext> Evaluate<TContext>(TContext context, IRule<TContext> rule)
        {
            var evaluation = new RuleEvaluation<TContext>(rule, rule.IsViolatedBy(context));

            return RuleEvaluationResult<TContext>.From(evaluation);
        }

        /// <summary>
        /// Validates a single rule against the given context, throwing if violated.
        /// </summary>
        /// <typeparam name="TContext">The context type the rule evaluates.</typeparam>
        /// <param name="context">The context to validate.</param>
        /// <param name="rule">The rule to validate.</param>
        /// <exception cref="RuleViolationException{TContext}">
        /// Thrown when the rule is violated for the provided context.
        /// </exception>
        public static void Validate<TContext>(TContext context, IRule<TContext> rule)
        {
            var result = Evaluate(context, rule);
            if (!result.IsValid)
                throw new RuleViolationException<TContext>(context, result);
        }

        /// <summary>
        /// Evaluates multiple rules against the given context.
        /// </summary>
        /// <typeparam name="TContext">The context type the rules evaluate.</typeparam>
        /// <param name="context">The context to evaluate.</param>
        /// <param name="rules">The rules collection.</param>
        /// <returns>An evaluation result aggregating all rule outcomes.</returns>
        /// <remarks>
        /// Returns a valid result immediately when the rules collection is empty.
        /// </remarks>
        public static RuleEvaluationResult<TContext> Evaluate<TContext>(TContext context, IReadOnlyCollection<IRule<TContext>> rules)
        {
            if (rules.Count == 0)
                return RuleEvaluationResult<TContext>.Valid;

            var evaluations = rules
                .Select(r => new RuleEvaluation<TContext>(r, r.IsViolatedBy(context)))
                .ToArray();

            return RuleEvaluationResult<TContext>.From(evaluations);
        }

        /// <summary>
        /// Validates multiple rules against the given context, throwing if any are violated.
        /// </summary>
        /// <typeparam name="TContext">The context type the rules evaluate.</typeparam>
        /// <param name="context">The context to validate.</param>
        /// <param name="rules">The rules collection.</param>
        /// <exception cref="RuleViolationException{TContext}">
        /// Thrown when one or more rules are violated for the provided context.
        /// </exception>
        public static void Validate<TContext>(TContext context, IReadOnlyCollection<IRule<TContext>> rules)
        {
            var result = Evaluate(context, rules);

            if (!result.IsValid)
                throw new RuleViolationException<TContext>(context, result);
        }

        /// <summary>
        /// Evaluates rules contained in a <see cref="RuleSet{TContext}"/> against the given context.
        /// </summary>
        /// <typeparam name="TContext">The context type the rules evaluate.</typeparam>
        /// <param name="context">The context to evaluate.</param>
        /// <param name="ruleSet">The rule set containing rules.</param>
        /// <returns>An evaluation result aggregating all rule outcomes.</returns>
        public static RuleEvaluationResult<TContext> Evaluate<TContext>(TContext context, RuleSet<TContext> ruleSet)
        {
            return Evaluate(context, ruleSet.Rules);
        }

        /// <summary>
        /// Validates rules contained in a <see cref="RuleSet{TContext}"/> against the given context, throwing if any are violated.
        /// </summary>
        /// <typeparam name="TContext">The context type the rules evaluate.</typeparam>
        /// <param name="context">The context to validate.</param>
        /// <param name="ruleSet">The rule set containing rules.</param>
        /// <exception cref="RuleViolationException{TContext}">
        /// Thrown when one or more rules are violated for the provided context.
        /// </exception>
        public static void Validate<TContext>(TContext context, RuleSet<TContext> ruleSet)
        {
            Validate(context, ruleSet.Rules);
        }
    }
}
