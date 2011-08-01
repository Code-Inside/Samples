using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using Dimebrain.TweetSharp.Fluent;
using System.Xml;
using System.Xml.Linq;
using SurfaceTwitter.Model;
using Dimebrain.TweetSharp.Fluent.Services;
using Dimebrain.TweetSharp;
using System;
using SurfaceTwitter.Util;

namespace SurfaceTwitter.Services
{
    public class TwitterService : ITwitterService
    {
        private NetworkCredential _credentials;
        private TwitterClientInfo _info;

        public bool IsAuthenticated { get; set; }

        public TwitterService(NetworkCredential credential)
        {
            this._info = new TwitterClientInfo();
            this._info.ClientName = "SurfaceTwitterApp";
            this._info.ClientVersion = "0.1";
            this._info.ClientUrl = "http://www.t-systems-mms.com/surface";

            this._credentials = credential;

            var twitter = FluentTwitter.CreateRequest()
                          .AuthenticateAs(this._credentials.UserName, this._credentials.Password)
                          .Statuses().Replies().AsXml();

            var response = twitter.Request();
            CheckXmlResponseForErrors(response);

            this.IsAuthenticated = true;
        }

        public void SendTweet(string message)
        {
            var twitter = FluentTwitter.CreateRequest(this._info)
                .AuthenticateAs(this._credentials.UserName, this._credentials.Password)
                .Statuses().Update(message)
                .AsXml();

            var response = twitter.Request();
            this.CheckXmlResponseForErrors(response);
        }

        public void SendTweet(string message, string picture)
        {
            var twitter = FluentTwitter.CreateRequest(this._info)
                .AuthenticateAs(this._credentials.UserName, this._credentials.Password)
                .Photos().PostPhoto(picture, SendPhotoServiceProvider.TwitPic)
                .Statuses().Update(message)
                .AsXml();

            var response = twitter.Request();
            this.CheckXmlResponseForErrors(response);
        }

        public IList<Tweet> GetTweets()
        {
            var twitter = FluentTwitter.CreateRequest()
                .AuthenticateAs(this._credentials.UserName, this._credentials.Password)
                .Statuses().OnFriendsTimeline().AsXml();

            var response = twitter.Request();
            this.CheckXmlResponseForErrors(response);

            XDocument doc = XDocument.Parse(response);
            List<Tweet> tweets = (from x in doc.Descendants("status")
                                  let u = x.Element("user")
                                  select new Tweet()
                                    {
                                        Id = int.Parse(x.Element("id").Value),
                                        CreatedOn = x.Element("created_at").Value.ParseDateTime(),
                                        Message = x.Element("text").Value,
                                        User = new User()
                                                   {
                                                       Name = u.Element("name").Value,
                                                       PictureUrl = u.Element("profile_image_url").Value
                                                   }
                                    }).ToList();

            return tweets;
        }



        private void CheckXmlResponseForErrors(string response)
        {
            XDocument doc = XDocument.Parse(response);

            var error = (from x in doc.Descendants("error")
                        select x).SingleOrDefault();
            if(error != null)
            {
                if (error.ToString().Contains("Could not authenticate you."))
                    throw new AuthenticationException("Could not authenticate you with Twitter");
            }
        }

        public void SendPicture(string picture)
        {
            var twitter = FluentTwitter.CreateRequest(this._info)
                .AuthenticateAs(this._credentials.UserName, this._credentials.Password)
                .Photos().PostPhoto(picture, SendPhotoServiceProvider.TwitPic)
                .Statuses().Update("")
                .AsXml();

            var response = twitter.Request();
            this.CheckXmlResponseForErrors(response);
        }

    }
}