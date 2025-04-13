using System.Reflection;
using System.Text;
namespace Converter;

public class CsvConverter<T>
{
    private readonly char _separator;
    private readonly string _dateTimeFormat;
    public CsvConverter(char separator = ',', string dateTimeFormat = "yyyy-MM-dd")
    {
        _separator = separator;
        _dateTimeFormat = dateTimeFormat;
    }

    public Stream ConvertToCsv(IEnumerable<T> data)
    {
        var stream = new MemoryStream();
        using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var header = string.Join(_separator, properties.Select(p => p.Name));
            writer.WriteLine(header);

            foreach (var item in data)
            {
                var values = new List<string>();
                foreach (var property in properties)
                {
                    var value = property.GetValue(item);
                    if (value is DateTime dateTime)
                    {
                        values.Add(dateTime.ToString(_dateTimeFormat));
                    }
                    else if (value is DateOnly dateOnly)
                    {
                        values.Add(dateOnly.ToString(_dateTimeFormat));
                    }
                    else
                    {
                        values.Add(value?.ToString() ?? string.Empty);
                    }
                }
                var line = string.Join(_separator, values);
                writer.WriteLine(line);
            }
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }
}