using DataProcessing.Services.FileReading.Interfaces;
using System.IO;

namespace DataProcessing.Services.FileReading
{
    public class FileReader : IFileReader
    {
        public async Task<IEnumerable<string>> ReadAllLinesAsync(string path)
        {
            var fileContent = await ReadToEndAsync(path);
            return fileContent.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
        }

        public async Task<string> ReadToEndAsync(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                var fileContext = await sr.ReadToEndAsync();
                return fileContext;
            }
        }
    }
}
