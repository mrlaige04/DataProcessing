using DataProcessing.Services.FileReading.Interfaces;

namespace DataProcessing.Services.FileReading
{
    public class FileReader : IFileReader
    {
        public async Task<IEnumerable<string>> ReadAllLinesAsync(string path, CancellationToken token = default)
        {
            var fileContent = await ReadToEndAsync(path, token);
            return fileContent.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
        }

        public async Task<string> ReadToEndAsync(string path, CancellationToken token = default)
        {
            using (StreamReader sr = new StreamReader(path, new FileStreamOptions
            {
                Access = FileAccess.Read,
                Mode = FileMode.Open,
            }))
            {
                var fileContext = await sr.ReadToEndAsync();
                return fileContext;
            }
        }
    }
}
