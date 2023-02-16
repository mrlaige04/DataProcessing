namespace DataProcessing.Services.FileReading.Interfaces
{
    public interface IFileReader
    {
        Task<IEnumerable<string>> ReadAsync(string path);
    }
}
