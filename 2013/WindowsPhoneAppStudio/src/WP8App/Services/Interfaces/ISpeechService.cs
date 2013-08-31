
namespace WPAppStudio.Services.Interfaces
{
    /// <summary>
    /// Interface for a speech service.
    /// </summary>
    public interface ISpeechService
    {
        /// <summary>
        /// Converts a text into a speech and pronounces it.
        /// </summary>
        /// <param name="text">The text to be pronounced.</param>		
        void TextToSpeech(string text);
    }
}
