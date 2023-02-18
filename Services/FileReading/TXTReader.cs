using DataProcessing.Services.FileReading.Interfaces;

namespace DataProcessing.Services.FileReading
{
    [Obsolete("Use FileReader instead")]
    public class TXTReader : IFileReader
    {
        public async Task<IEnumerable<string>> ReadAllLinesAsync(string path, CancellationToken token = default)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                var fileContext = await sr.ReadToEndAsync();
                return fileContext.Split("\n\r", StringSplitOptions.RemoveEmptyEntries);
            }
        }

        [Obsolete("This method is not implemented")]
        public Task<string> ReadToEndAsync(string path, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
    }
}
