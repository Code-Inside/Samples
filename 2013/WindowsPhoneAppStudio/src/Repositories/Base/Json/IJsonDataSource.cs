using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace WPAppStudio.Repositories.Base
{
    /// <summary>
    /// Interface for a data source based on JSON.
    /// </summary>
    public interface IJsonDataSource
    {
        /// <summary>
        /// Gets the JSON data from a specified URL.
        /// </summary>
        /// <typeparam name="T">The generic type.</typeparam>
        /// <param name="url">The URL.</param>
        /// <returns>The data retrieved.</returns>
        Task<T> LoadRemote<T>(string url);

        /// <summary>
        /// Gets the JSON data from a specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>The data retrieved as dynamic object.</returns>		
        Task<JArray> LoadRemote(string url);
    }
}
