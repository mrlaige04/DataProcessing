using DataProcessing.Services.FileReading.Interfaces;

namespace DataProcessing.Services.FileReading
{
    public class TXTReader : IFileReader
    {
        public async Task<IEnumerable<string>> ReadAsync(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                var fileContext = await sr.ReadToEndAsync();
                return fileContext.Split("\n\r", StringSplitOptions.RemoveEmptyEntries);
            }
        }
    }
}
