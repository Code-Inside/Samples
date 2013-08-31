
namespace WPAppStudio.Services.Interfaces
{
    /// <summary>
    /// Interface for the Share service.
    /// </summary>
    public interface IShareService
    {
        /// <summary>
        /// Executes the Share service.
        /// </summary>
        /// <param name="title">The title shared.</param>
        /// <param name="message">The message shared.</param>
        /// <param name="link">The link shared.</param>
		/// <param name="type">Share type.</param>
        void Share(string title, string message, string link = "", string type = "");
    }
}
