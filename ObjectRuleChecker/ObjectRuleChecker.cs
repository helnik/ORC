using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ObjectRuleChecker.Interfaces;

namespace ObjectRuleChecker
{
    public class ObjectRuleChecker<T> : IObjectRuleChecker<T> where T : class 
    {
        private readonly List<ValidationRule<T>> validationRules;
     
        public ObjectRuleChecker(IEnumerable<ValidationRule<T>> rules)
        {
            if (rules == null) throw new ArgumentNullException(nameof(validationRules));
            validationRules = rules.ToList();
        }

        /// <summary>
        /// Creates a new ObjectRuleChecker with no rules specified
        /// </summary>
        public static ObjectRuleChecker<T> Create => new ObjectRuleChecker<T>(new List<ValidationRule<T>>());

        /// <summary>
        /// Adds a new rule to the existing ones
        /// </summary>
        /// <param name="newRule">a new rule for validation</param>
        public IObjectRuleChecker<T> IsValidWhen(Expression<Func<T, bool>> newRule)
        {
            if (newRule == null) throw new ArgumentNullException(nameof(newRule));
            validationRules.Add(new ValidationRule<T>(newRule));
            return this;
        }

        /// <summary>
        /// Adds a new rule to the existing ones
        /// </summary>
        /// <param name="newRule">a new rule for validation</param>
        public IObjectRuleChecker<T> IsNotValidWhen(Expression<Func<T, bool>> newRule)
        {
            if (newRule == null) throw new ArgumentNullException(nameof(newRule));
            var notNewRule = Expression.Lambda<Func<T, bool>>(Expression.Not(newRule.Body),newRule.Parameters);
            return IsValidWhen(notNewRule);
        }

        /// <summary>
        /// Adds a specified exception to the preceding rule, does nothing if no rule is provided
        /// </summary>
        /// /// <param name="ex">a specified exception</param>
        public IObjectRuleChecker<T> Throws(Exception ex)
        {
            if (ex == null) throw new ArgumentNullException(nameof(ex));
            var lastRule = validationRules.Last();
            if (lastRule == null) return this;
            lastRule.AddException(ex);
            return this;
        }

        /// <summary>
        /// Makes the preceding rule blocking, meaning it will instruct validator to stop on failure
        /// </summary>
        public IObjectRuleChecker<T> Block()
        {
            var lastRule = validationRules.Last();
            if (lastRule == null) return this;
            lastRule.MakeBlocking();
            return this;
        }

        /// <summary>
        /// Checks if all specified rules apply to given object. Throws first exception specified if rule does not apply.
        /// Stops checking in the first encountered rule that not applies
        /// </summary>
        public bool CheckAll(T toCheck)
        {
            bool isValid = false;
            foreach (var validationRule in validationRules)
            {
                isValid = validationRule.Applies(toCheck);
                if (isValid) continue;
                if (validationRule.Exception != null) throw validationRule.Exception;
                break;
            }
            return isValid;
        }

        /// <summary>
        /// Checks given object against every rule. Does not throw, saves the exception, if specified, in result
        /// Stops if a failed rule is blocking
        /// </summary>
        /// <param name="toCheck"></param>
        /// <returns></returns>
        public List<IOrcResult> GetValidationResults(T toCheck)
        {
            var results = new List<IOrcResult>();
            foreach (var validationRule in validationRules)
            {
                bool isValid = validationRule.Applies(toCheck);
                var ex = isValid ? null : validationRule.Exception;
                results.Add(new OrcResult(validationRule.ToString(), isValid, ex));
                if (validationRule.IsBlockingRule) break;
            }
            return results;
        }
    }
}
