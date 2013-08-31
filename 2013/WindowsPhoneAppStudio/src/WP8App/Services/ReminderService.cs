using Microsoft.Phone.Scheduler;
using WPAppStudio.Services.Interfaces;

namespace WPAppStudio.Services
{
    /// <summary>
    /// Implementation of a reminder service.
    /// </summary>
    public class ReminderService : IReminderService
    {
        /// <summary>
        /// Indicates if the reminder is scheduled or not.
        /// </summary>
        /// <param name="name">The name of the remainder.</param>
        /// <returns>A boolean value indicating if the reminder is scheduled.</returns>
        public bool IsScheduled(string name)
        {
            ScheduledAction schedule = ScheduledActionService.Find(name);
            return schedule != null && schedule.IsScheduled;
        }
    }
}