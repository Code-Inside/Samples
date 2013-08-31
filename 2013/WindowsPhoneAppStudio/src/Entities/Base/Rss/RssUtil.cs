using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace WPAppStudio.Entities.Base
{
    public class RssUtil
    {	
        public static string GetSummary(SyndicationItem item)
        {
            var summary = item.Summary != null && item.Summary.Text != null ?
                          (item.Summary.Text) :
                          string.Empty;
            if (string.IsNullOrEmpty(summary) && item.Content is TextSyndicationContent)
                summary = ((TextSyndicationContent)item.Content).Text;

            return DeleteImages(summary);
        }
		
        public static string GetImage(SyndicationItem item, bool searchInSummary)
        {
            string feedDataImage = null;
            try
            {
                foreach (SyndicationLink link in from link in item.Links
                                                 let linkImagen = link.Uri.AbsoluteUri.ToLower()
                                                 where linkImagen.Contains(".jpg") || linkImagen.Contains(".jpeg") || linkImagen.Contains(".png") || linkImagen.Contains(".gif")
                                                 select link)
                {
                    feedDataImage = link.Uri.AbsoluteUri;
                }

                if (string.IsNullOrEmpty(feedDataImage))
                    foreach (var link in from link in item.ElementExtensions
                                         let linkImagen = link.OuterName.ToLower()
                                         where linkImagen.Contains(".jpg") || linkImagen.Contains(".jpeg") || linkImagen.Contains(".png") || linkImagen.Contains(".gif")
                                         select link)
                    {
                        feedDataImage = link.OuterName;
                    }

                if (string.IsNullOrEmpty(feedDataImage) && searchInSummary)
                {
                    if (item.Summary != null && item.Summary.Text.Contains("<img "))
                    {
                        string aux = item.Summary.Text.Substring(item.Summary.Text.IndexOf("<img ", StringComparison.Ordinal));
                        aux = aux.Substring(0, aux.IndexOf(">", StringComparison.Ordinal));
                        aux = aux.Substring(aux.IndexOf("src=", StringComparison.Ordinal));

                        if (aux.Contains("https")) aux = aux.Substring(aux.IndexOf("https", StringComparison.Ordinal));
                        else if (aux.Contains("http"))
                        {
                            aux = aux.Substring(aux.IndexOf("http", StringComparison.Ordinal));
                            if (aux.Contains(".jpg"))
                                feedDataImage = aux.Substring(0, aux.LastIndexOf(".jpg", StringComparison.Ordinal) + 4);
                            else if (aux.Contains(".png"))
                                feedDataImage = aux.Substring(0, aux.LastIndexOf(".png", StringComparison.Ordinal) + 4);
                            else if (aux.Contains(".jpeg"))
                                feedDataImage = aux.Substring(0, aux.LastIndexOf(".", StringComparison.Ordinal) + 5);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("There was an error getting image from RSS. Error: {0}", ex);
            }
            return feedDataImage;
        }

        public static string GetExtraImage(SyndicationItem rssItem)
        {
            string feedMediaImage = null;
            try
            {
                SyndicationElementExtensionCollection extensions = rssItem.ElementExtensions;

                // Read extension items in order to find images: png, jpg preference to the bigger.
                if (extensions != null && extensions.Count > 0)
                {
                    var listImages = new List<XElement>();
                    // media:thumbnail and itunes
                    if (extensions.Any(p => p.OuterName == "thumbnail" || p.OuterName == "image"))
                    {
                        var extensionImages = new List<SyndicationElementExtension>(extensions.Where(p => p.OuterName == "thumbnail" || p.OuterName == "image"));
                        listImages.AddRange(extensionImages.Select(aux => new XElement(aux.GetObject<XElement>())));
                    }

                    if (listImages != null)
                    {
                        // Apply filter to get higher images.
                        var filteredImages = new List<XElement>();

                        foreach (XElement elem in listImages)
                        {
                            // 1.- Ask for .png - checking that the URL is an absolute URL.
                            if (elem.Attribute("url") != null && elem.Attribute("url").Value.ToLower().Contains("http")
                                                                && elem.Attribute("url").Value.ToLower().Contains("png"))
                            {
                                filteredImages.Add(elem);
                            }
                            else if (elem.Value.ToLower().Contains("http") && elem.Value.ToLower().Contains("png")) // in itunes the image url is not an attribute.
                            {
                                filteredImages.Add(elem);
                            }
                        }
                        if (filteredImages.Count == 0)
                        {
                            // 2.- Ask for .jpg - checking that the URL is an absolute URL.
                            foreach (XElement elem in listImages)
                            {
                                if (elem.Attribute("url") != null && elem.Attribute("url").Value.ToLower().Contains("http")
                                                                && elem.Attribute("url").Value.ToLower().Contains("jpg"))
                                {
                                    filteredImages.Add(elem);
                                }
                                else if (elem.Value.ToLower().Contains("http") && elem.Value.ToLower().Contains("jpg")) // in itunes the image url is not an attribute.
                                {
                                    filteredImages.Add(elem);
                                }
                            }
                        }
                        // 3.- Get the bigger.
                        if (filteredImages != null)
                        {
                            var compareImagesBySize = new Comparison<XElement>(CompareImagesBySize);
                            filteredImages.Sort(compareImagesBySize);
                            XElement resultImage = filteredImages.FirstOrDefault();
                            if (resultImage != null)
                                feedMediaImage = (resultImage.Attribute("url") != null) ? resultImage.Attribute("url").Value : resultImage.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("There was an error getting extra image from RSS. Error: {0}", ex);
            }

            return feedMediaImage;
        }

        public static string GetItemFeedLink(SyndicationItem item)
        {
			var link = string.Empty;
            try
            {
                if (item.Links != null && item.Links.Any())
                    link = item.Links.FirstOrDefault().Uri.AbsoluteUri;
                else
                {
                    // Read extensions in order to find feed link info, supported cases: wfw:commentRss, c9:feed.
                    if (item.ElementExtensions != null && item.ElementExtensions.Count > 0)
                    {
                        // commentRss
                        if (item.ElementExtensions.Count(p => p.OuterName == "commentRss") > 0)
                        {
                            SyndicationElementExtension ext = item.ElementExtensions.First(p => p.OuterName == "commentRss");
                            var elem = ext.GetObject<XElement>();
                            link = elem.Value;
                        }
                        // feed
                        if (link == string.Empty && item.ElementExtensions.Any(p => p.OuterName == "feed"))
                        {
                            SyndicationElementExtension ext = item.ElementExtensions.First(p => p.OuterName == "feed");
                            var elem = ext.GetObject<XElement>();
                            link = elem.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("There was an error getting feed item from RSS. Error: {0}", ex);
            }

            return link;
        }

        public static string GetMediaVideoURL(SyndicationItem item)
        {
            string mediaVideoUrl = string.Empty;

            try
            {
                SyndicationElementExtensionCollection extensions = item.ElementExtensions;

                // Read extension items in order to find videos -> media:content.
                if (extensions != null && extensions.Count > 0)
                {
                    // media:group & media:content's
                    if (extensions.Count(p => p.OuterName == "group") == 1)
                    {
                        SyndicationElementExtension extensionGroup = extensions.First(p => p.OuterName == "group");
                        if (extensionGroup != null)
                        {
                            XNode groupNode = XDocument.ReadFrom(extensionGroup.GetReader());
                            XDocument doc = XDocument.Parse(groupNode.ToString());
                            XNamespace media = "http://search.yahoo.com/mrss/";
                            // Get content subitems inside group.
                            var query = (from contentElem in doc.Descendants(media + "content")
                                         select contentElem);
                            List<XElement> listContent = query.ToList();

                            if (listContent.Count > 0)
                            {
                                // Filter by type "video/mp4" | "video/x-ms-wmv"
                                var videoElems = new List<XElement>(listContent.Where(c => c.Attribute("type").Value == "video/mp4" && c.Attribute("url").Value.ToUpper().Contains("HIGH")));

                                if (!videoElems.Any())
                                {
                                    videoElems = new List<XElement>(listContent.Where(c => c.Attribute("type").Value == "video/mp4"));
                                    if (!videoElems.Any())
                                    {
                                        videoElems = new List<XElement>(listContent.Where(c => c.Attribute("type").Value == "video/x-ms-wmv"));
                                    }
                                }
                                if (videoElems.Any() && videoElems.First().Attribute("url") != null)
                                {
                                    mediaVideoUrl = videoElems.First().Attribute("url").Value;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("There was an error getting media video from RSS. Error: {0}", ex);
            }

            return mediaVideoUrl;
        }

        private static int CompareImagesBySize(XElement elem1, XElement elem2)
        {
            int size1 = elem1.Attribute("width") != null ? int.Parse(elem1.Attribute("width").Value) : 0;
            if (size1 == 0)
            {
                size1 = elem1.Attribute("height") != null ? int.Parse(elem1.Attribute("height").Value) : 0;
            }

            int size2 = elem2.Attribute("width") != null ? int.Parse(elem2.Attribute("width").Value) : 0;
            if (size2 == 0)
            {
                size2 = elem1.Attribute("height") != null ? int.Parse(elem2.Attribute("height").Value) : 0;
            }

            return size2.CompareTo(size1);
        }

        private static string DeleteImages(string html)
        {
            const string pattern = @"<(img)[^>]*>(?<content>[^<]*)";
            var regex = new Regex(pattern);
            var m = regex.Match(html);
            if (m.Success)
            {
                string img = m.Value;
                html = html.Replace(img, string.Empty);
            }

            return html;
        }
    }
}
