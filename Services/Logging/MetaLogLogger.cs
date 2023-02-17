using DataProcessing.Models.Logging.Abstract;
using DataProcessing.Services.Logging.Interfaces;
using Microsoft.Extensions.Logging;

namespace DataProcessing.Services.Logging
{
    public class MetaLogLogger : ILogging
    {

        public MetaLogLogger()
        {
            
        }

        public async Task LogToFile(ILogData data, string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                await sw.WriteAsync(data.ToString());
            }
        }
    }
}
