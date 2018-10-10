using enTT.ZS.Remainder.Shared.Enum;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace enTT.ZS.Remainder.Quartz
{
    /// <summary>
    /// This is ADO job store based Quartz scheduler, there must be a Config section in config file 
    /// with name "quartz" which contains all DB and other information
    /// </summary>
    public class Quartz
    {
        /// <summary>
        /// Get ADO jobstore Quartz scheduler
        /// </summary>
        /// <returns></returns>
        private static IScheduler getScheduler()
        {
            var schedulerFactory = new StdSchedulerFactory(ConfigurationManager.GetSection("quartz") as NameValueCollection);
            IScheduler scheduler = schedulerFactory.GetScheduler().Result;
            scheduler.Start().Wait();

            return scheduler;
        }

        private static async Task CreateRemainder<T>(ZSJobsAndTriggerInfo remainderDetails, RemainderInfoKey key) where T : IJob
        {
            IJobDetail job = null;
            ITrigger trigger = null;
            JobKey jobKey = JobKey.Create(remainderDetails.jobIdentityKey);

            job = JobBuilder.Create<T>().
               WithIdentity(jobKey)
               .UsingJobData(key.ToString(), remainderDetails.jobData_Info)
               .Build();

            //trigger = TriggerBuilder.Create()
            //.WithIdentity(JobandTrigger.TriggerIdentityKey)
            //.StartNow()
            //.WithSimpleSchedule(x => x.WithIntervalInSeconds(JobandTrigger.ScheduleIntervalInSec).WithRepeatCount(1)
            ////.RepeatForever()
            //)
            //.Build();

            trigger = (ISimpleTrigger)TriggerBuilder.Create()
            .WithIdentity(remainderDetails.TriggerIdentityKey)
            .StartAt(DateBuilder.FutureDate(remainderDetails.ScheduleIntervalInSec, IntervalUnit.Second)) // use DateBuilder to create a date in the future
            .ForJob(jobKey) // identify job with its JobKey
            .Build();

            await getScheduler().ScheduleJob(job, trigger);
        }



        /// <summary>
        /// Create a remainder for workflow
        /// </summary>
        /// <typeparam name="T">IJob class, this the callback class when the remainder is being triggered</typeparam>
        /// <param name="triggerInterval">The timeSpan (when we want to trigger this)</param>
        /// <param name="RemainderInfo">details about the remainder, you can give a Json string for this, this data can be retrived from the IJob T callback method</param>
        /// <returns></returns>
        public static Guid CreateWorkflowRemainder<T>(TimeSpan triggerInterval,string RemainderInfo ) where T : IJob
        {
            Guid Id = Guid.NewGuid();
            ZSJobsAndTriggerInfo remInfo = new ZSJobsAndTriggerInfo();
            remInfo.jobIdentityKey = Id.ToString();
            remInfo.TriggerIdentityKey = remInfo.jobIdentityKey;
            remInfo.jobData_Info = RemainderInfo;
            remInfo.ScheduleIntervalInSec = Convert.ToInt32(triggerInterval.TotalSeconds);

            CreateRemainder<T>(remInfo, RemainderInfoKey.WorkflowRemainderInfo).GetAwaiter().GetResult();

            return Id;
        }

        public static Guid CreateWorkflowRemainder<T>(DateTime triggerTime, string RemainderInfo) where T : IJob
        {
            if (triggerTime < DateTime.Now)
                throw new ArgumentException("Trigget time must be a future time");

            Guid Id = Guid.NewGuid();
            ZSJobsAndTriggerInfo remInfo = new ZSJobsAndTriggerInfo();
            remInfo.jobIdentityKey = Id.ToString();
            remInfo.TriggerIdentityKey = remInfo.jobIdentityKey;
            remInfo.jobData_Info = RemainderInfo;
            remInfo.ScheduleIntervalInSec = Convert.ToInt32(triggerTime.Subtract(DateTime.Now).TotalSeconds);

            CreateRemainder<T>(remInfo, RemainderInfoKey.WorkflowRemainderInfo).GetAwaiter().GetResult();

            return Id;
        }

        public static void GetAwaiterClass()
        {

            getScheduler();

        }
    }
}
