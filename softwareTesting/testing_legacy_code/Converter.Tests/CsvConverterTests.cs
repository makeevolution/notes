namespace Converter.Tests;
using Xunit;
public class ConverterApprovalTests
{
    [Fact]
    public void Test()
    {
        
    }
    [Fact]
    public Task Should_return_csv()
    {
        var employees = new List<Employee>
        {
            new Employee { Id = 1, Name = "John Doe", BirthDate = new DateOnly(1990, 1, 1) },
            new Employee { Id = 2, Name = "Jane Smith", BirthDate = new DateOnly(1985, 5, 15) },
        };
        var csvConverter = new CsvConverter<Employee>();
        // act
        var stream = csvConverter.ConvertToCsv(employees);

        // assert/verify
        return Verify(stream);
    }

    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateOnly BirthDate { get; set; }
    }
}
