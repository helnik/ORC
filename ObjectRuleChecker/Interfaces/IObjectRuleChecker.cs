using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ObjectRuleChecker.Interfaces
{
    public interface IObjectRuleChecker<T> where T : class
    {
        IObjectRuleChecker<T> IsValidWhen(Expression<Func<T, bool>> newRule);
        IObjectRuleChecker<T> IsNotValidWhen(Expression<Func<T, bool>> newRule);
        IObjectRuleChecker<T> Throws(Exception ex);
        IObjectRuleChecker<T> Block();
        bool CheckAll(T toCheck);
        List<IOrcResult> GetValidationResults(T toCheck);
    }
}