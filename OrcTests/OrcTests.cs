using System;
using System.Linq;
using ObjectRuleChecker;
using Xunit;

namespace OrcTests
{
    public class OrcTests
    {
        [Fact]
        public void ReferenceEqualsTest()
        {
            var orc = ObjectRuleChecker<Employee>.Create
                .IsValidWhen(e => e.Age == 30)
                .IsNotValidWhen(e => e.SalaryAmount > 2000);

            var orc2 = orc.IsValidWhen(e => e.IsFullTime);
            Assert.Same(orc, orc2);
        }

        [Fact]
        public void CallWithBoolResult()
        {
            bool isValid = ObjectRuleChecker<Employee>.Create
                .IsValidWhen(e => e != null)
                .IsValidWhen(e => e.Age == 30)
                .IsNotValidWhen(e => e.SalaryAmount > 2000)
                .CheckAll(new Employee());
            Assert.True(isValid);
        }

        [Fact]
        public void CallWithResultsNoExceptions()
        {
            var results = ObjectRuleChecker<Employee>.Create
                .IsNotValidWhen(e => e == null)
                .IsValidWhen(e => e.Age == 30)
                .IsNotValidWhen(e => e.SalaryAmount > 2000)
                .IsNotValidWhen(e => e.IsFullTime) //expect 1 error
                .GetValidationResults(new Employee());
            Assert.True(results.Count == 4);
            Assert.True(results.Where(r => !r.IsSuccess).ToList().Count == 1);
        }

        [Fact]
        public void CallWithResultsAndExceptions()
        {
            string exMessage = "Invalid employee contract";
            var results = ObjectRuleChecker<Employee>.Create
                .IsNotValidWhen(e => e == null)
                .IsValidWhen(e => e.Age == 30)
                .IsNotValidWhen(e => e.SalaryAmount > 2000)
                .IsNotValidWhen(e => e.IsFullTime).Throws(new ArgumentException(exMessage))
                .GetValidationResults(new Employee());

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
            try
            {
                bool isValid = ObjectRuleChecker<Employee>.Create
                    .IsValidWhen(e => e.Age == 40).Throws(new ArgumentException(exMessage))
                    .IsNotValidWhen(e => e.SalaryAmount > 2000)
                    .IsNotValidWhen(e => e.IsFullTime) 
                    .CheckAll(new Employee());
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
            Employee employee = null;
            bool isValid = ObjectRuleChecker<Employee>.Create
                .IsNotValidWhen(e => e == null)
                .IsNotValidWhen(e => e.SalaryAmount > 2000)
                .IsNotValidWhen(e => e.IsFullTime)
                .CheckAll(employee);

            Assert.False(isValid);
        }

        [Fact]
        public void CallWithBlocking()
        {
            Employee employee = null;
            var results = ObjectRuleChecker<Employee>.Create
                .IsNotValidWhen(e => e == null).Block()
                .IsNotValidWhen(e => e.SalaryAmount > 2000)
                .IsNotValidWhen(e => e.IsFullTime)
                .GetValidationResults(employee);

            Assert.True(results.Count == 1);
        }

        [Fact]
        public void CallWithBlockingAndThrow()
        {
            Employee employee = null;
            var results = ObjectRuleChecker<Employee>.Create
                .IsNotValidWhen(e => e == null).Block().Throws(new ArgumentNullException(nameof(employee)))
                .IsNotValidWhen(e => e.SalaryAmount > 2000)
                .IsNotValidWhen(e => e.IsFullTime)
                .GetValidationResults(employee);

            Assert.True(results.Count == 1);
            Assert.True(results.FirstOrDefault()?.Exception != null);
        }
    }
}
