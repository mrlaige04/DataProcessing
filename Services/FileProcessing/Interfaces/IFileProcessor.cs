using DataProcessing.Models.Logging;
using DataProcessing.Models;

namespace DataProcessing.Services.FileProcessing.Interfaces
{
    public interface IFileProcessor
    {
        Task<Output> ProcessFileAsync(string filePath, MetaLogData metaData);
    }
}
