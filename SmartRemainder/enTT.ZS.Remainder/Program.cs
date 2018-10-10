using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2
{

    class JobsAndTriggers
    {
        public string jobIdentityKey { get; set; }
        public string TriggerIdentityKey { get; set; }
        public string jobData_MethodName { get; set; }
        public int ScheduleIntervalInSec { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<JobsAndTriggers> LstobjJobsAndTriggers = new List<JobsAndTriggers>();
            LstobjJobsAndTriggers.Add(new JobsAndTriggers { jobIdentityKey = "JOB1", TriggerIdentityKey = "TRIGGER1", jobData_MethodName = "JOB1_METHOD()", ScheduleIntervalInSec = 10 });
            LstobjJobsAndTriggers.Add(new JobsAndTriggers { jobIdentityKey = "JOB2", TriggerIdentityKey = "TRIGGER2", jobData_MethodName = "JOB2_METHOD()", ScheduleIntervalInSec = 15 });
            //LstobjJobsAndTriggers.Add(new JobsAndTriggers { jobIdentityKey = "JOB5", TriggerIdentityKey = "TRIGGER5", jobData_MethodName = "JOB5_METHOD()", ScheduleIntervalInSec = 150 });

            Console.WriteLine("writing");
            TestDemoJob1(LstobjJobsAndTriggers).GetAwaiter().GetResult();

            Console.ReadKey();
        }

        public static async Task TestDemoJob1(List<JobsAndTriggers> lstJobsAndTriggers)
        {
            IScheduler scheduler;
            IJobDetail job = null;
            ITrigger trigger = null;

            var schedulerFactory = new StdSchedulerFactory(ConfigurationManager.GetSection("quartz") as NameValueCollection);

            scheduler = schedulerFactory.GetScheduler().Result;
            scheduler.Start().Wait();

            foreach (var JobandTrigger in lstJobsAndTriggers)
            {
                JobKey jobKey = JobKey.Create(JobandTrigger.jobIdentityKey);

                job = JobBuilder.Create<DemoJob1>().
                   WithIdentity(jobKey)
                   .UsingJobData("MethodName", JobandTrigger.jobData_MethodName)
                   .Build();

                //trigger = TriggerBuilder.Create()
                //.WithIdentity(JobandTrigger.TriggerIdentityKey)
                //.StartNow()
                //.WithSimpleSchedule(x => x.WithIntervalInSeconds(JobandTrigger.ScheduleIntervalInSec).WithRepeatCount(1)
                ////.RepeatForever()
                //)
                //.Build();

                trigger = (ISimpleTrigger)TriggerBuilder.Create()
                .WithIdentity(JobandTrigger.TriggerIdentityKey)
                .StartAt(DateBuilder.FutureDate(JobandTrigger.ScheduleIntervalInSec, IntervalUnit.Second)) // use DateBuilder to create a date in the future
                .ForJob(jobKey) // identify job with its JobKey
                .Build();

                await scheduler.ScheduleJob(job, trigger);

            }
        }
    }

    public class DemoJob1 : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            //simple task, the job just prints current datetime in console
            //return Task.Run(() => {
            //    //Console.WriteLine(DateTime.Now.ToString());
            //});

            JobKey key = context.JobDetail.Key;

            JobDataMap dataMap = context.JobDetail.JobDataMap;
            string MethodName = dataMap.GetString("MethodName");
            Console.WriteLine(DateTime.Now.ToString() + " =>" + MethodName);

            return Task.FromResult(0);
        }
    }
}