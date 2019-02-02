using System;
using ObjectRuleChecker.Interfaces;

namespace ObjectRuleChecker
{
    public class OrcResult : IOrcResult
    {
        public bool IsSuccess { get; }
        public string Rule { get; }
        public Exception Exception { get; }

        public OrcResult(string rule, bool isSuccess, Exception ex = null)
        {
            rule.EnsureContext("No rule provided for the ValidationResult");
            Rule = rule;
            IsSuccess = isSuccess;
            Exception = ex;
        }

        public override string ToString()
        {
            string result = IsSuccess ? "Passed" : "Failed";
            return $"Rule: {Rule} {result}";
        }
    }
}
