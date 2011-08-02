using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SurfaceTwitter.Services;
using System.Security.Authentication;
using SurfaceTwitter.Model;

namespace SurfaceTwitterTest
{
    /// <summary>
    /// Summary description for TwitterServiceTests
    /// </summary>
    [TestClass]
    public class TwitterServiceTests
    {
        public TwitterServiceTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private NetworkCredential GetCredentials()
        {
            return new NetworkCredential("", "");
        }

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void TwitterService_Invalid_Authentication_Throws_Exception()
        {
            ITwitterService twitter = new TwitterService(new NetworkCredential("", ""));
        }

        [TestMethod]
        public void TwitterService_GetTweets_Returns_Values_NEEDS_AUTHENTICATION()
        {
            ITwitterService twitter = new TwitterService(this.GetCredentials());
            List<Tweet> tweets = twitter.GetTweets().ToList();

            foreach (Tweet item in tweets)
            {
                Assert.AreNotEqual(0, item.Id);
                Assert.AreNotEqual(DateTime.MinValue, item.CreatedOn);
                Assert.AreNotEqual("", item.Message);
                Assert.AreNotEqual("", item.User);
            }
        }

        [TestMethod]
        public void TwitterService_SendTweet_Works_NEEDS_AUTHENTICATION()
        {
            ITwitterService twitter = new TwitterService(this.GetCredentials());
            twitter.SendTweet(Guid.NewGuid().ToString());
        }

        [TestMethod]
        public void TwitterService_SendTweet_With_Picture_Works_NEEDS_AUTHENTICATION()
        {

            ITwitterService twitter = new TwitterService(this.GetCredentials());
            twitter.SendTweet(Guid.NewGuid().ToString(), "C:\\twitter.jpg");
        }

        [TestMethod]
        public void TwitterService_SendPicture_Works_NEEDS_AUTHENTICATION()
        {

            ITwitterService twitter = new TwitterService(this.GetCredentials());
            twitter.SendPicture("C:\\twitter.jpg");
        }
    }
}
