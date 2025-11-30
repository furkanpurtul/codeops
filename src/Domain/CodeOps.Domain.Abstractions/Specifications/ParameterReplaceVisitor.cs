using System.Linq.Expressions;

namespace CodeOps.Domain.Abstractions.Specifications
{
    internal sealed class ParameterReplaceVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _source;
        private readonly ParameterExpression _target;

        public ParameterReplaceVisitor(ParameterExpression source, ParameterExpression target)
        {
            _source = source;
            _target = target;
        }


        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node == _source) return _target;
            return base.VisitParameter(node);
        }
    }
}
