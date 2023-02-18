namespace DataProcessing.Services.FileReading.Interfaces
{
    public interface IFileReader
    {
        Task<IEnumerable<string>> ReadAllLinesAsync(string path, CancellationToken token = default);
        Task<string> ReadToEndAsync(string path, CancellationToken token = default);
    }
}
