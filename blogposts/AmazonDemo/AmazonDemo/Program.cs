using System;
using System.Collections.Generic;
using System.Text;

using AmazonDemo.com.amazon.webservices;
namespace AmazonDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            AWSECommerceService Service = new AWSECommerceService();

            ItemSearch itemSearch = new ItemSearch();
            ItemSearchRequest request = new ItemSearchRequest();

            // Keys
            itemSearch.AWSAccessKeyId = ""; // YOUR ACCESS KEY!
            itemSearch.AssociateTag = "meinkleinerbl-21"; // YOUR ASSOCIATE TAG!
            
            // Suchbereich eingrenzen
            request.ResponseGroup = new string[] { "Medium", "Offers" };
            request.SearchIndex = "Blended";
            request.Keywords = "Potter"; // Suchwort
            
            request.ItemPage = "1";
            request.Count = "25"; // Maximum per Page

            itemSearch.Request = new ItemSearchRequest[] { request };

            try
            {
                ItemSearchResponse response = Service.ItemSearch(itemSearch);

                Items[] itemsArray = response.Items;

                foreach (Items items in itemsArray)
                {
                    Console.WriteLine("Total Pages; " + items.TotalPages);
                    Console.WriteLine("Total Results; " + items.TotalResults);

                    if (items != null)
                    {
                        if (items.Item != null)
                        {
                            for (int i = 0; i < items.Item.Length; i++)
                            {
                                Console.WriteLine();
                                Console.WriteLine("Title: " + items.Item[i].ItemAttributes.Title);

                                if (items.Item[i].Offers.TotalOffers != "0")
                                {
                                    if (items.Item[i].Offers.Offer[0].OfferListing[0].Price != null)
                                    {
                                        Console.WriteLine("Price: " + items.Item[i].Offers.Offer[0].OfferListing[0].Price.FormattedPrice);
                                    }
                                }
                                Console.WriteLine("SalesRank: " + items.Item[i].SalesRank);
                                Console.WriteLine("SalesRank: " + items.Item[i].ASIN);
                                Console.WriteLine("Description: " + items.Item[i].ItemAttributes.Label);

                            }

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }   
    }
}
