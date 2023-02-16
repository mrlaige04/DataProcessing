using CsvHelper;
using CsvHelper.Configuration;
using DataProcessing.Services.FileReading.Interfaces;
using System.Globalization;

namespace DataProcessing.Services.FileReading
{
    public class CSVReader : IFileReader
    {
        public async Task<IEnumerable<string>> ReadAsync(string path)
        {
            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };

            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, configuration))
            {
                var records = csv.GetRecords<string>();
                return records;
            }
        }
    }
}
