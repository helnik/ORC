using System;

namespace ObjectRuleChecker.Interfaces
{
    public interface IOrcResult
    {
        bool IsSuccess { get; }
        string Rule { get; }
        Exception Exception { get; }
    }
}
