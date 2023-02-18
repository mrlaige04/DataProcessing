using DataProcessing.Models;
using DataProcessing.Models.Logging;

namespace DataProcessing.Services.FileProcessing.Interfaces
{
    public interface IFileProcessor
    {
        Task<Output> ProcessFileAsync(string filePath, MetaLogData metaData, CancellationToken token = default);
    }
}
