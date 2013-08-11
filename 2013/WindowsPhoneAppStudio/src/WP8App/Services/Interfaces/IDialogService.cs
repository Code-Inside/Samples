
namespace WPAppStudio.Services.Interfaces
{
    /// <summary>
    /// Interface for dialog service.
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Shows a message in the dialog.
        /// </summary>
		/// <param name="message">The message to show.</param>
        void Show(string message);

        /// <summary>
        /// Shows a message with a caption in the dialog.
        /// </summary>
        /// <param name="message">The message to show.</param>
        /// <param name="caption">The caption of the dialog.</param>
        void Show(string message, string caption);
    }
}
