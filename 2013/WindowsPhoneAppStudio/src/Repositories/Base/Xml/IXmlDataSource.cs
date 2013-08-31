using System.Threading.Tasks;

namespace WPAppStudio.Repositories.Base
{
    /// <summary>
    /// Interface for a data source based on XML.
    /// </summary>
    public interface IXmlDataSource
    {
        /// <summary>
        /// Gets the XML data from a specified URL.
        /// </summary>
        /// <typeparam name="T">The generic type.</typeparam>
        /// <param name="url">The URL.</param>
        /// <returns>The data retrieved.</returns>
        Task<T> LoadRemote<T>(string url);
    }
}
