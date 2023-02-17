using DataProcessing.Models.Logging.Abstract;

namespace DataProcessing.Services.Logging.Interfaces
{
    public interface ILogging
    {
        Task LogToFile(ILogData data, string path);
    }
}
