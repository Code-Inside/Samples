
namespace WPAppStudio.Services.Interfaces
{
    /// <summary>
    /// Interface for a reminder service.
    /// </summary>
    public interface IReminderService
    {
        /// <summary>
        /// Indicates if the reminder is scheduled or not.
        /// </summary>
        /// <param name="name">The name of the remainder.</param>
        /// <returns>A boolean value indicating if the reminder is scheduled.</returns>
        bool IsScheduled(string name);
    }
}
