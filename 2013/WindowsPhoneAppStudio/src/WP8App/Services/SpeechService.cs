using WPAppStudio.Entities.Base;
using WPAppStudio.Services.Interfaces;
using System;
using System.Globalization;
using System.Diagnostics;
using System.Linq;
using Windows.Phone.Speech.Synthesis;
using WPAppStudio.Localization;

namespace WPAppStudio.Services
{
    /// <summary>
    /// Implementation of a speech service.
    /// </summary>
    public class SpeechService : ISpeechService
    {
        /// <summary>
        /// Converts a text into a speech and pronounces it.
        /// </summary>
        /// <param name="text">The text to be pronounced.</param>
        public async void TextToSpeech(string text)
        {
            try
            {
                if (!string.IsNullOrEmpty(text))
                {
                    var synth = new SpeechSynthesizer();

                    var language = CultureInfo.CurrentCulture.ToString();

                    var voices = InstalledVoices.All.Where(v => v.Language == language).OrderByDescending(v => v.Gender);

                    const VoiceGender gender = VoiceGender.Female;

                    synth.SetVoice(voices.FirstOrDefault(v => v.Gender == gender));

                    await synth.SpeakTextAsync(HtmlUtil.CleanHtml(text));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("{0}{1}. {2}{3}", AppResources.SpeechError, text, AppResources.Error, ex.ToString());
            }
        }
    }
}
