namespace MvcLocalization.WebApp.Infrastructure
{
    /// <summary>
    /// Interface to access the session
    /// </summary>
    public interface ICookieRepository
    {
        /// <summary>
        /// Adds an element to the cookie
        /// </summary>
        /// <typeparam name="T">Type of the element</typeparam>
        /// <param name="key">CookieKey for Store</param>
        /// <param name="data">Data to store</param>
        void AddElement(CookieKey key, string data);

        /// <summary>
        /// Get a specific element of the cookie based on the type and key
        /// </summary>
        /// <typeparam name="T">Type of the element</typeparam>
        /// <param name="key">Specific SessionKey</param>
        /// <returns>The element from the session</returns>
        string GetElement(CookieKey key);

        /// <summary>
        /// Checks if a element is in the cookie based on the key and type
        /// </summary>
        /// <typeparam name="T">Type of the element</typeparam>
        /// <param name="key">CookieKey for lookup</param>
        /// <returns>true if element is found and has the correct type, false if the key doesn´t exists or it has the wrong type</returns>
        bool HasElement(CookieKey key);

        /// <summary>
        /// Deletes a element in the session based on the key
        /// </summary>
        /// <param name="key">SessionKey for lookup</param>
        void DeleteElement(CookieKey key);

        /// <summary>
        /// Updates and element
        /// </summary>
        /// <typeparam name="T">Type of the element</typeparam>
        /// <param name="key">SessionKey for lookup</param>
        /// <param name="data">New data for storing</param>
        /// <exception cref="InvalidOperationException">Exception is thrown if element doesn´t exists</exception>
        void UpdateElement(CookieKey key, string data);
    }
}