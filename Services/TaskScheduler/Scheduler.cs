using DataProcessing.Models.Logging;
using DataProcessing.Services.AppManagement;
using DataProcessing.Services.Logging;
using Quartz;
using Quartz.Impl;

namespace DataProcessing.Services.TaskScheduler
{
    public class Scheduler
    {
        private IScheduler scheduler;

        public async Task InitScheduler(AppManager app)
        {
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            scheduler = await schedulerFactory.GetScheduler();


            IJobDetail job = JobBuilder.Create<MetaLogLogger>()
                .WithIdentity("myJob", "group1")
                .Build();
            job.JobDataMap["app"] = app; // metadata input

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("midnightTrigger", "logger")
                .WithDailyTimeIntervalSchedule(x => x
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 0)))
                .Build();

            // TODO: FIX - This method calls each time when executing Start() 
            await scheduler.ScheduleJob(job, trigger);    
        }

        public async Task StartScheduler(CancellationToken token = default)
        {
            await scheduler.Start(token);
        }
    }
}
