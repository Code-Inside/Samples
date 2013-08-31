using System.Linq;
using System.Xml.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WPAppStudio.Entities.Base
{
    /// <summary>
    /// Representation of a YouTube video.
    /// </summary>
    public class YouTubeVideo : BindableBase
    {
		private const string YoutubeWatchBaseUrl = "http://www.youtube.com/watch?v=";
		
        private string _title;
        /// <summary>
        /// Gets/Sets the title of the YouTube video.
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
				SetProperty(ref _title, value);                
            }
        }

        private string _videoUrl;
        /// <summary>
        /// Gets/Sets the video URL of the YouTube video.
        /// </summary>
        public string VideoUrl
        {
            get { return _videoUrl; }
            set
            {
				SetProperty(ref _videoUrl, value);                   
            }
        }

        private string _videoImageUrl;
        /// <summary>
        /// Gets/Sets the video image URL of the YouTube video.
        /// </summary>
        public string VideoImageUrl
        {
            get { return _videoImageUrl; }
            set
            {
				SetProperty(ref _videoImageUrl, value);
            }
        }
		
        private string _videoId;
        /// <summary>
        /// Gets/Sets the identifier of the YouTube video.
        /// </summary>
        public string VideoId
        {
            get
            {
                if (!string.IsNullOrEmpty(VideoUrl))
                {
                    var parsed = VideoUrl.Split('/');
                    _videoId = parsed[parsed.Length - 1];
                }
                return _videoId;
            }
            set 
			{ 
				SetProperty(ref _videoId, value);
			}
        }
		
		public string ExternalUrl
        {
            get { return YoutubeWatchBaseUrl + VideoId; }
        }

        private string _summary;
        /// <summary>
        /// Gets/Sets the summary of the YouTube video.
        /// </summary>
        public string Summary
        {
            get { return _summary; }
            set
            {
				SetProperty(ref _summary, value);
            }
        }
		
        public YouTubeVideo()
        {
        }
		
		public YouTubeVideo(XNamespace atomns, XElement entry, XNamespace medians)
        {
            var id = entry.Element(atomns.GetName("id")) != null && entry.Element(atomns.GetName("id")).Value != null
                ? entry.Element(atomns.GetName("id")).Value.Split(':').Last()
                : string.Empty;
		    VideoUrl = "http://gdata.youtube.com/feeds/api/videos/" + id;
            VideoImageUrl = (from thumbnail in entry.Descendants(medians.GetName("thumbnail"))
                select thumbnail.Attribute("url").Value).FirstOrDefault();
            Title = entry.Element(atomns.GetName("title")) != null
                ? entry.Element(atomns.GetName("title")).Value
                : string.Empty;
            Summary = entry.Element(atomns.GetName("summary")) != null
                ? entry.Element(atomns.GetName("summary")).Value
                : string.Empty;
            if (string.IsNullOrEmpty(Summary))
                Summary = entry.Element(atomns.GetName("content")) != null
                    ? entry.Element(atomns.GetName("content")).Value
                    : string.Empty;
            if (string.IsNullOrEmpty(Summary))
                Summary = Title;
        }
    }
}
