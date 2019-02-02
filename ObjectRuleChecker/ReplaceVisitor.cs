using System;
using System.Linq.Expressions;

namespace ObjectRuleChecker
{
    internal class ReplaceVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression fromParameter;
        private readonly ParameterExpression toParameter;

        internal ReplaceVisitor(ParameterExpression fromParameter, ParameterExpression toParameter)
        {
            this.fromParameter = fromParameter;
            this.toParameter = toParameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node.Equals(fromParameter) ? toParameter : base.VisitParameter(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            return node.Expression is ConstantExpression ? Expression.Parameter(node.Type, node.Member.Name) : base.VisitMember(node);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            return node.Operand.Type == fromParameter.Type ? Expression.Parameter(node.Operand.Type, toParameter.Name) : base.VisitUnary(node);
        }

        internal static Expression Replace(Expression target, ParameterExpression from, ParameterExpression to)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (from == null) throw new ArgumentNullException(nameof(from));
            if (to == null) throw new ArgumentNullException(nameof(to));

            return new ReplaceVisitor(from, to).Visit(target);
        }
    }
}
