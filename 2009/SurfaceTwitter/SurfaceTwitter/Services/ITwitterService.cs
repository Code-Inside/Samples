using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SurfaceTwitter.Model;

namespace SurfaceTwitter.Services
{
    public interface ITwitterService
    {
        void SendTweet(string message);
        void SendTweet(string message, string picture);
        void SendPicture(string picture);
        IList<Tweet> GetTweets();

        bool IsAuthenticated { get; set; }
    }
}