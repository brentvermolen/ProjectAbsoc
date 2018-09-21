using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Jobs
{
    public class StayOnline : IJob
    {
        Task IJob.Execute(IJobExecutionContext context)
        {
            System.Threading.Thread.Sleep(1000);

            return Task.FromResult(0);
        }
    }
}
