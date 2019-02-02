using System;
using System.Collections.Generic;
using System.Linq;
using ObjectRuleChecker;
using Xunit;

namespace OrcTests
{
    public class ImmutableOrcTests
    {
        [Fact]
        public void ReferenceEqualsTest()
        {
            var validationRules = new List<ValidationRule<Employee>>
            {
                new ValidationRule<Employee>(e => e != null),
                new ValidationRule<Employee>(e => e.IsFullTime),
                new ValidationRule<Employee>(e => e.Person != null)
            };
            var orc = new ImmutableObjectRuleChecker<Employee>(validationRules);

            var orc2 = orc.IsNotValidWhen(e => e.SalaryAmount == 2000);
            Assert.NotSame(orc, orc2);
        }

        [Fact]
        public void CallWithBoolResult()
        {
            var validationRules = new List<ValidationRule<Employee>>
            {
                new ValidationRule<Employee>(e => e != null),
                new ValidationRule<Employee>(e => e.Age == 30),
                new ValidationRule<Employee>(e => !(e.SalaryAmount > 2000))
            };
            var orc = new ImmutableObjectRuleChecker<Employee>(validationRules);
            bool isValid = orc.CheckAll(new Employee());
            Assert.True(isValid);
        }

        [Fact]
        public void CallWithResultsNoExceptions()
        {
            var validationRules = new List<ValidationRule<Employee>>
            {
                new ValidationRule<Employee>(e => e != null),
                new ValidationRule<Employee>(e => e.Age == 30),
                new ValidationRule<Employee>(e => !(e.SalaryAmount > 2000)),
                new ValidationRule<Employee>(e => !e.IsFullTime)
            };
            var orc = new ImmutableObjectRuleChecker<Employee>(validationRules);
            var results = orc.GetValidationResults(new Employee());
            Assert.True(results.Count == 4);
            Assert.True(results.Where(r => !r.IsSuccess).ToList().Count == 1);
        }

        [Fact]
        public void CallWithResultsAndExceptions()
        {
            string exMessage = "Invalid employee contract";
            var validationRules = new List<ValidationRule<Employee>>
            {
                new ValidationRule<Employee>(e => e != null),
                new ValidationRule<Employee>(e => e.Age == 30),
                new ValidationRule<Employee>(e => !(e.SalaryAmount > 2000)),
                new ValidationRule<Employee>(e => !e.IsFullTime).AddException(new ArgumentException(exMessage))
            };

            var orc = new ImmutableObjectRuleChecker<Employee>(validationRules);
            var results = orc.GetValidationResults(new Employee());

            Assert.True(results.Count == 4);
            var faultedRes = results.Where(r => !r.IsSuccess).ToList();
            Assert.True(faultedRes.Count == 1);
            var faulted = faultedRes.FirstOrDefault(fr => fr.Exception != null);
            Assert.Equal(exMessage, faulted.Exception.Message);
            Assert.True(faulted.Exception is ArgumentException);
        }

        [Fact]
        public void CallWithThrow()
        {
            string exMessage = "Employee not in correct age";
            var validationRules = new List<ValidationRule<Employee>>
            {
                new ValidationRule<Employee>(e => e != null),
                new ValidationRule<Employee>(e => e.Age == 40).AddException(new ArgumentException(exMessage)),
                new ValidationRule<Employee>(e => !(e.SalaryAmount > 2000)),
                new ValidationRule<Employee>(e => !e.IsFullTime)
            };
            try
            {
                var orc = new ImmutableObjectRuleChecker<Employee>(validationRules);
                orc.GetValidationResults(new Employee());
            }
            catch (ArgumentException aex)
            {
                Assert.Equal(aex.Message, exMessage);
            }
            catch
            {
                Assert.True(false); //fail it we have some other exception
            }
        }

        [Fact]
        public void CallWithNull()
        {
           
            var validationRules = new List<ValidationRule<Employee>>
            {
                new ValidationRule<Employee>(e => e != null),
                new ValidationRule<Employee>(e => !(e.SalaryAmount > 2000)),
                new ValidationRule<Employee>(e => !e.IsFullTime)
            };
            var orc = new ImmutableObjectRuleChecker<Employee>(validationRules);
            bool isValid = orc.CheckAll(null);

            Assert.False(isValid);
        }

        [Fact]
        public void CallWithBlocking()
        {
            var validationRules = new List<ValidationRule<Employee>>
            {
                new ValidationRule<Employee>(e => e != null).MakeBlocking(),
                new ValidationRule<Employee>(e => !(e.SalaryAmount > 2000)),
                new ValidationRule<Employee>(e => !e.IsFullTime)
            };

            var orc = new ImmutableObjectRuleChecker<Employee>(validationRules);
            var results = orc.GetValidationResults(null);

            Assert.True(results.Count == 1);
        }

        [Fact]
        public void CallWithBlockingAndThrow()
        {
            Employee employee = null;
            var validationRules = new List<ValidationRule<Employee>>
            {
                new ValidationRule<Employee>(e => e != null).MakeBlocking().AddException(new ArgumentNullException(nameof(employee))),
                new ValidationRule<Employee>(e => !(e.SalaryAmount > 2000)),
                new ValidationRule<Employee>(e => !e.IsFullTime)
            };

            var orc = new ImmutableObjectRuleChecker<Employee>(validationRules);
            var results = orc.GetValidationResults(null);

            Assert.True(results.Count == 1);
            Assert.True(results.FirstOrDefault()?.Exception != null);
        }
    }
}
