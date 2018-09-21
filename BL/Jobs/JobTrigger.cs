using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Jobs
{
    public class JobTrigger
    {
        public static async Task StartAsync()
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();

            scheduler.Start();

            //IJobDetail jobWeeklyReview = JobBuilder.Create<WeeklyReview>().Build();
            //ITrigger triggerWeeklyReview = TriggerBuilder.Create()
            //.WithIdentity("trigger1", "group1")
            //.StartNow()
            //.WithSimpleSchedule(x => x
            //    .WithInterval(TimeSpan.FromDays(7))
            //    .RepeatForever())
            //.Build();

            //scheduler.ScheduleJob(jobWeeklyReview, triggerWeeklyReview);

            //IJobDetail jobCleanActors = JobBuilder.Create<CleanActors>().Build();
            //ITrigger triggerCleanActors = TriggerBuilder.Create()
            //    .WithIdentity("trigger2", "group2")
            //    .StartNow()
            //    .WithSimpleSchedule(x => x
            //        .WithInterval(TimeSpan.FromDays(7))
            //        .RepeatForever())
            //    .Build();

            //scheduler.ScheduleJob(jobCleanActors, triggerCleanActors);

            IJobDetail jobStayOnline = JobBuilder.Create<StayOnline>().Build();
            ITrigger triggerStayOnline = TriggerBuilder.Create()
                .WithIdentity("trigger3", "group3")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithInterval(TimeSpan.FromMinutes(2))
                    .RepeatForever())
                .Build();

            scheduler.ScheduleJob(jobStayOnline, triggerStayOnline);
        }
    }
}
