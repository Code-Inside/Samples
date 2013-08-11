using System;
using System.Linq;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.ServiceModel.Syndication;

namespace WPAppStudio.Entities.Base
{
    /// <summary>
    /// Representation of the results in a Flickr search.
    /// </summary>
    public class FlickrSearchResult : BindableBase
    {
        private string _id;
        /// <summary>
        /// Gets/Sets the identifier of the Flickr.
        /// </summary>
        public string Id
        {
            get { return _id; }
            set
            {
                SetProperty(ref _id, value);
            }
        }

        private string _title;
        /// <summary>
        /// Gets/Sets the title of the Flickr.
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                SetProperty(ref _title, value);                
            }
        }

        private string _summary;
        /// <summary>
        /// Gets/Sets the summary of the Flickr.
        /// </summary>
        public string Summary
        {
            get { return _summary; }
            set
            {
				SetProperty(ref _summary, value);                
            }
        }

        private string _image;
        /// <summary>
        /// Gets/Sets the image of the Flickr.
        /// </summary>
        public string Image
        {
            get { return _image; }
            set
            {
				SetProperty(ref _image, value);                 
            }
        }

        private DateTime _published;
        /// <summary>
        /// Gets/Sets the publish date of the Flickr.
        /// </summary>
        public DateTime Published
        {
            get { return _published; }
            set
            {
				SetProperty(ref _published, value);    
            }
        }
		
        /// <summary>
        /// Initializes a new instance of <see cref="FlickrSearchResult" /> class.
        /// </summary>
        public FlickrSearchResult() { }

		/// <summary>
        /// Initializes a new instance of <see cref="FlickrSearchResult" /> class.
        /// </summary>
        /// <param name="item">A valid SyndicationItem.</param>
        public FlickrSearchResult(SyndicationItem item)
            : this()
        {
            Id = item.Id;
            Title = item.Title != null ? HttpUtility.HtmlDecode(item.Title.Text) : string.Empty;
		    Summary = RssUtil.GetSummary(item);
            Image = RssUtil.GetImage(item, true);
            Published = item.PublishDate.DateTime;
        }
    }
}