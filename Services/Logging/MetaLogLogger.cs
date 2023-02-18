using DataProcessing.Models.Logging.Abstract;
using DataProcessing.Services.AppManagement;
using DataProcessing.Services.Logging.Interfaces;
using Quartz;

namespace DataProcessing.Services.Logging
{
    public class MetaLogLogger : ILogging, IJob
    {

        public MetaLogLogger()
        {

        }

        public async Task Execute(IJobExecutionContext context)
        {
            var app = context.MergedJobDataMap["app"] as AppManager;
            if (app != null) await app.DoAtMidnight();
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
