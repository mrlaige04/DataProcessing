using CsvHelper;
using CsvHelper.Configuration;
using DataProcessing.Services.FileReading.Interfaces;
using System.Globalization;

namespace DataProcessing.Services.FileReading
{
    [Obsolete("Use FileReader instead")]
    public class CSVReader : IFileReader
    {
        public async Task<IEnumerable<string>> ReadAllLinesAsync(string path, CancellationToken token = default)
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
        public Task<string> ReadToEndAsync(string path, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
    }
}
