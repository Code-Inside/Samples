namespace WPAppStudio.Services.Interfaces
{
    /// <summary>
    /// Interface for a Lock Screen service.
    /// </summary>
    public interface ILockScreenService
    {
        /// <summary>
        /// Sets the Lock Screen for the application.
        /// </summary>
        /// <param name="lockKey">The lock key.</param>
        void SetLockScreen(string lockKey);
    }
}
