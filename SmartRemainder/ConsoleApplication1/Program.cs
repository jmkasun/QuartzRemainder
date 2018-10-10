using enTT.ZS.Remainder.Shared.Enum;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime time = new DateTime(2018, 10, 09, 18, 11, 30);
            Guid id = new Guid();

            //Console.WriteLine(DateTime.Now.ToString());
           // id =  enTT.ZS.Remainder.Quartz.Quartz.CreateWorkflowRemainder<QJob>(time, "Remainder =>"+ time.ToString("HH:mm:ss fff"));

            enTT.ZS.Remainder.Quartz.Quartz.GetAwaiterClass();
            Console.WriteLine(DateTime.Now.ToString() + " =>" + id);

            Console.ReadKey();
        }
    }

    class QJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            JobKey key = context.JobDetail.Key;

            JobDataMap dataMap = context.JobDetail.JobDataMap;
            string MethodName = dataMap.GetString(RemainderInfoKey.WorkflowRemainderInfo.ToString());
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss fff") + " =>" + MethodName);

            return Task.FromResult(0);
        }
    }
}
