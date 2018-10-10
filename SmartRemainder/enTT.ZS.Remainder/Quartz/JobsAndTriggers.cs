    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace enTT.ZS.Remainder.Quartz
{

    class ZSJobsAndTriggerInfo
    {
        public string jobIdentityKey { get; set; }
        public string TriggerIdentityKey { get; set; }
        public string jobData_Info { get; set; }
        public int ScheduleIntervalInSec { get; set; }
    }
}
