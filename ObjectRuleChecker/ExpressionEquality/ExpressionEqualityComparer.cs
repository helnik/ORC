using System.Collections.Generic;
using System.Linq.Expressions;

namespace ObjectRuleChecker.ExpressionEquality
{
    public class ExpressionEqualityComparer : IEqualityComparer<Expression>
    {
        public static readonly ExpressionEqualityComparer Instance = new ExpressionEqualityComparer();

        public bool Equals(Expression x, Expression y)
        {
            return new ExpressionComparisonVisitor(x, y).AreEqual;
        }

        public int GetHashCode(Expression obj)
        {
            return new ExpressionHashCodeCalculationVisitor(obj).HashCode;
        }
    }
}
