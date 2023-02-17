namespace DataProcessing.Services.FileReading.Interfaces
{
    public interface IFileReader
    {
        Task<IEnumerable<string>> ReadAllLinesAsync(string path);
        Task<string> ReadToEndAsync(string path);
    }
}
