using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using DataProcessing.Services.FileReading.Interfaces;
using System.Globalization;

namespace DataProcessing.Services.FileReading
{
    public class CSVReader : IFileReader
    {
        public async Task<IEnumerable<string>> ReadAllLinesAsync(string path)
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

        [Obsolete("This method is not implemented")]
        public Task<string> ReadToEndAsync(string path)
        {
            throw new NotImplementedException();
        }
    }
}
