using DataProcessing.Services.FileReading.Interfaces;

namespace DataProcessing.Services.FileReading
{
    public class FileReader : IFileReader
    {
        public async Task<IEnumerable<string>> ReadAsync(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                var fileContext = await sr.ReadToEndAsync();
                return fileContext.Split("\r\n", StringSplitOptions.RemoveEmptyEntries).ToList();
            }
        }
    }
}
