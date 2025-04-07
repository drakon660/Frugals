// See https://aka.ms/new-console-template for more information

using Frugals;
using Frugals.Perf;

//BenchmarkDotNet.Running.BenchmarkRunner.Run<FrugalBenchmarks>();

// var list = new FrugalTaskList<int>();
// list.Add(Create(1));
// list.Add(Create(2));
// await list.WaitAllAndGetResultsAsync();
// Console.ReadKey();
//
//
// static Task<int> Create(int count)
// {
//     Console.WriteLine(count);
//     return Task.FromResult(1);
// }
var dept1Employees = new List<Employee> { 
    new Employee { Id = 101, Name = "John", Salary = 50000 },
    new Employee { Id = 102, Name = "Alice", Salary = 60000 }
};

var dept2Employees = new List<Employee> { 
    new Employee { Id = 101, Name = "John", Salary = 5000 }, // Bonus
    new Employee { Id = 103, Name = "Bob", Salary = 55000 }
};

var m = new MergeList<Employee>((x,y)=>x.Id == y.Id, (employee, employee1) => new Employee
{
    Id = employee.Id,
    Name = employee.Name,
    Salary = employee.Salary,
},  dept1Employees, dept2Employees);

Console.ReadKey();

record Employee
{
    public int Id { get; init; }
    public string Name { get; init; }
    public decimal Salary { get; init; }
}