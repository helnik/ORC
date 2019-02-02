using System.Runtime.InteropServices.ComTypes;

namespace OrcTests
{
    public class Person
    {
        public string Name { get; set; } = "John";
        public string LastName { get; set; } = "Doe";
    }

    public class Employee
    {
        public Person Person { get; set; } = new Person();
        public bool IsFullTime { get; set; } = true;
        public int Age { get; set; } = 30;
        public int SalaryAmount { get; set; } = 1000;
        public string Company { get; set; } = "EvilCorp";
    }

    
}
