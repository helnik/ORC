using System;
using System.Linq.Expressions;

namespace ObjectRuleChecker
{
    public class ValidationRule<T> where T : class
    {
        public Exception Exception { get; private set; }
        public bool IsBlockingRule { get; private set; } 
        private readonly Lazy<string> expressionString;
        private readonly Lazy<Func<T, bool>> predicate;

        public ValidationRule(Expression<Func<T, bool>> expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            predicate = new Lazy<Func<T, bool>>(expression.Compile);
            expressionString = new Lazy<string>(() => GetExpressionToString(expression));
        }

        public bool Applies(T toValidate) => predicate.Value(toValidate);

        public ValidationRule<T> AddException(Exception ex)
        {
            Exception = ex;
            return this;
        }

        public ValidationRule<T> MakeBlocking() 
        {
            IsBlockingRule = true;
            return this;
        }

        public override string ToString() => expressionString.Value;

        private static string GetExpressionToString(Expression<Func<T, bool>> expression)
        {
            var tReplacer = Expression.Parameter(typeof(T), $"{typeof(T).Name}");
            //var rv = new ReplaceVisitor(expression.Parameters[0], tReplacer);
            //return rv.Visit(expression.Body).ToString();
            return ReplaceVisitor.Replace(expression.Body, expression.Parameters[0], tReplacer).ToString();
        }
    }
}
