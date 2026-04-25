using PropertyManagementSystem.Infrastructure.Jobs;

namespace PropertyManagementSystem.Infrastructure.Scheduling
{
    public static class HangfireJobs
    {
        public static void Register(IRecurringJobManager jobs)
        {
            jobs.AddOrUpdate<RentReminderJob>(
                "rent-reminder",
                job => job.Execute(),
                Cron.Daily(9)
            );

            jobs.AddOrUpdate<LeaseExpiryJob>(
                "lease-expiry",
                job => job.Execute(),
                Cron.Daily(8)
            );
        }
    }
}