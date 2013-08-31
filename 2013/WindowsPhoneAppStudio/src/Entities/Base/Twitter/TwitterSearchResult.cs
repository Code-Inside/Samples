using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.ServiceModel.Syndication;
using Newtonsoft.Json.Linq;

namespace WPAppStudio.Entities.Base
{
    /// <summary>
    /// Representation of a Twitter search.
    /// </summary>
    public class TwitterSearchResult : INotifyPropertyChanged
    {
        private string _author;
        /// <summary>
        /// Gets/Sets the author of the tweet.
        /// </summary>
        public string Author
        {
            get
            {
                return _author;
            }
            set
            {
                if (_author != value)
                {
                    _author = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _tweet;
        /// <summary>
        /// Gets/Sets the tweet message.
        /// </summary>
        public string Tweet
        {
            get
            {
                return _tweet;
            }
            set
            {
                if (_tweet != value)
                {
                    _tweet = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private DateTime _publishDate;
        /// <summary>
        /// Gets/Sets the publish date of the tweet.
        /// </summary>
        public DateTime PublishDate
        {
            get
            { return _publishDate; }
            set
            {
                if (_publishDate != value)
                {
                    _publishDate = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _id;
        /// <summary>
        /// Gets/Sets the identifier of the tweet.
        /// </summary>
        public string Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _avatarUrl;
        /// <summary>
        /// Gets/Sets the avatar URL.
        /// </summary>
        public string AvatarUrl
        {
            get { return _avatarUrl; }
            set
            {
                if (_avatarUrl != value)
                {
                    _avatarUrl = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Event launched when a property of the TwitterSearchResult object has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="TwitterSearchResult" /> class.
        /// </summary>
        public TwitterSearchResult()
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="TwitterSearchResult" /> class.
        /// </summary>
		/// <param name="item">A valid SyndicationItem.</param>
        public TwitterSearchResult(SyndicationItem item)
            : this()
        {
            Author = item.Authors[0].Name;
            Id = GetTweetId(item.Id);
            Tweet = item.Title.Text;
            PublishDate = item.PublishDate.DateTime.ToLocalTime();
            AvatarUrl = item.Links[1].Uri.AbsoluteUri;
        }
		
		/// <summary>
        /// Initializes a new instance of <see cref="TwitterSearchResult" /> class.
        /// </summary>
        /// <param name="jObject">A valid json from twitter api.</param>
        public TwitterSearchResult(JToken jObject)
            : this()
        {
            var value = JObject.Parse(jObject.ToString());
            var user = JObject.Parse(value["user"].ToString());

            Author = user["screen_name"].ToString();
            Tweet = value["text"].ToString();
            PublishDate = DateTime.ParseExact(value["created_at"].ToString(), "ddd MMM dd HH:mm:ss %K yyyy", CultureInfo.InvariantCulture.DateTimeFormat);
            AvatarUrl = user["profile_image_url"].ToString();
        }

        private string GetTweetId(string twitterId)
        {
            var parts = twitterId.Split(":".ToCharArray());

            return parts[2];
        }
    }
}
