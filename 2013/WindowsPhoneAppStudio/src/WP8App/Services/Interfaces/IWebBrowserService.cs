
namespace WPAppStudio.Services.Interfaces
{
    /// <summary>
    /// Interface for a web browser service.
    /// </summary>
    public interface IWebBrowserService
    {
        /// <summary>
        /// Pins a tile to start page.
        /// </summary>
        void OpenBrowser(string url);
    }
}
