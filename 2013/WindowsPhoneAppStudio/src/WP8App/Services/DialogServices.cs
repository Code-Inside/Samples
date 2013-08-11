using WPAppStudio.Services.Interfaces;
using System.Windows;

namespace WPAppStudio.Services
{
    /// <summary>
    /// Implementation of a dialog service.
    /// </summary>
    public class DialogService : IDialogService
    {
        /// <summary>
        /// Shows a message in the dialog.
        /// </summary>
		/// <param name="message">The message to show.</param>
        public void Show(string message)
        {
            MessageBox.Show(message);
        }

        /// <summary>
        /// Shows a message with a caption in the dialog.
        /// </summary>
        /// <param name="message">The message to show.</param>
        /// <param name="caption">The caption of the dialog.</param>
        public void Show(string message, string caption)
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK);
        }
    }
}
