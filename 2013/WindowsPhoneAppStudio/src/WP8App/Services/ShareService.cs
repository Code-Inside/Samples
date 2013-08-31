using Microsoft.Phone.Tasks;
using WPAppStudio.Entities.Base;
using WPAppStudio.Localization;
using WPAppStudio.Services.Interfaces;

namespace WPAppStudio.Services
{
    /// <summary>
    /// Implementation of the Share service.
    /// </summary>
    public class ShareService : IShareService
    {
        /// <summary>
        /// Executes the Share service.
        /// </summary>
        /// <param name="title">The title shared.</param>
        /// <param name="message">The message shared.</param>
        /// <param name="link">The link shared.</param>
		/// <param name="type">Share type.</param>
        public void Share(string title, string message, string link = "", string type = "")
        {
            title = string.IsNullOrEmpty(title) ? string.Empty : HtmlUtil.CleanHtml(title);
            message = string.IsNullOrEmpty(message) ? string.Empty : HtmlUtil.CleanHtml(message);
            var linkUri = string.IsNullOrEmpty(link) ? new System.Uri(AppResources.HomeUrl) : new System.Uri(link, System.UriKind.Absolute);
            var shareLinkTask = new ShareLinkTask
            {
                Title = title,
                Message = message,
                LinkUri = linkUri
            };

            shareLinkTask.Show();
        }
    }
}
