using System.Threading.Tasks;

namespace WPAppStudio.Repositories.Base
{
    /// <summary>
    /// Interface for a YouTube data source.
    /// </summary>
    public interface IYoutubeDataSource
    {
        /// <summary>
        /// Gets the YouTube data from a specified URL.
        /// </summary>
        /// <typeparam name="T">The generic type.</typeparam>
        /// <param name="url">The URL.</param>
        /// <returns>The data retrieved.</returns>
        Task<T> LoadRemote<T>(string url);
    }
}


