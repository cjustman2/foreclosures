using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Net;
using System.IO;

namespace foreclosures.Utilities
{
    public class PageParser
    {
      
        public List<Listing> ClientListing(string pageData)
        {



            string[] text = pageData.Split(new string[] { "<div class=\"Freeform CenterZone\">" }, StringSplitOptions.RemoveEmptyEntries);

            string[] tables = text[4].Split(new string[] { "<table border=\"0\" cellspacing=\"1\" cellpadding=\"1\" width=\"100%\">" }, StringSplitOptions.RemoveEmptyEntries);


            List<Listing> addresses = new List<Listing>();


            string table = tables[2].Trim().Substring(0, tables[2].IndexOf("</tbody>") + 1).Replace("&nbsp;", "");

            XDocument doc = XDocument.Parse(table);

      
            int i = 0;

            foreach (XElement element in doc.Descendants("tr"))
            {

                if (i > 0)
                {

                    List<XElement> tds = element.Descendants("td").ToList();
                    if (tds.Count > 1)
                    {
                        string file = "http://city.milwaukee.gov"; ;
                        List<XElement> hrefs = tds[1].Descendants("a").ToList();
                        if (hrefs.Count > 1)
                        {
                            file += hrefs[0].Attribute("href").Value;
                        }

                        XElement ahref = tds[1].Descendants("a").First();


                        addresses.Add(new Listing { ListingAddress = ahref.Value + " Milwaukee, WI", Image = file });

                    }
                }

                i++;
            }

            return addresses;
        }


        public List<Listing> ExtendedListing(string pageData)
        {
            string[] text = pageData.Split(new string[] { "<div class=\"Freeform CenterZone\">" }, StringSplitOptions.RemoveEmptyEntries);

            string[] tables = text[4].Split(new string[] { "<table border=\"0\" cellspacing=\"1\" cellpadding=\"1\" width=\"100%\">" }, StringSplitOptions.RemoveEmptyEntries);


            List<Listing> addresses = new List<Listing>();


            string table = tables[1].Trim().Substring(0, tables[1].IndexOf("</tbody>") + 1).Replace("&nbsp;", "").Replace("</tbody", "</tbody>");

            XDocument doc = XDocument.Parse(table);

            int i = 0;

            foreach (XElement element in doc.Descendants("tr"))
            {

                if (i > 0)
                {

                    List<XElement> tds = element.Descendants("td").ToList();
                    if (tds.Count > 1)
                    {
                        string file = "http://city.milwaukee.gov"; ;
                        List<XElement> hrefs = tds[0].Descendants("a").ToList();
                        if (hrefs.Count > 1)
                        {
                            file += hrefs[0].Attribute("href").Value;
                        }

                        XElement ahref = tds[0].Descendants("a").First();


                        addresses.Add(new Listing { ListingAddress = ahref.Value + " Milwaukee, WI", Image = file });

                    }
                }

                i++;
            }

            return addresses;
        }

        public List<Listing> DodgeCounty(string pageData)
        {

            int start = pageData.IndexOf("<div id=\"ctl00_content_Screen\">");
            string html = pageData.Substring(start);

            List<Listing> addresses = new List<Listing>();

            string table = html.Trim().Substring(0, html.IndexOf("</div>") + 6).Replace("<hr>", "").Replace("&nbsp;", "");


            XDocument doc = XDocument.Parse(table);



            foreach (XElement element in doc.Descendants("ul"))
            {

                List<XElement> lis = element.Descendants("li").ToList();

                for (int i = 0; i < lis.Count; i++)
                {
                    List<XElement> hrefs = lis[i].Descendants("a").ToList();

                    if (hrefs.Count > 0)
                    {
                        string file = "http://www.co.dodge.wi.us/";
                        int end = lis[i].Value.ToString().IndexOf(')') + 1;
                        string addr = lis[i].Value.Remove(0, end).Trim();
                        int dash = addr.LastIndexOf('-');

                        Listing address = new Listing();
                        if (dash > 4)
                        {
                            address.ListingAddress = addr.Substring(0, dash);
                           
                        }
                        else
                        {
                            address.ListingAddress = addr;
                        }

                     

                        file += hrefs[0].Attribute("href").Value;
                        address.Image = file;
                        addresses.Add(address);
                    }

                   
                }
           
            }
            return addresses;
        }



        public List<Listing> WashingtonCounty(string pageData)
        {
            int start = pageData.IndexOf("<div id=\"sales\" class=\"editable\">");
            string html = pageData.Substring(start);

            List<Listing> addresses = new List<Listing>();

            string table = html.Trim().Substring(0, html.IndexOf("</div>") + 6).Replace("<hr>", "");


            XDocument doc = XDocument.Parse(table);

            int i = 0;

            foreach (XElement element in doc.Descendants("p"))
            {


                List<XElement> hrefs = element.Descendants("a").ToList();
                if (hrefs.Count > 0)
                {
                    string file = "http://www.washingtoncountysheriffwi.org"; ;


                    file += hrefs[0].Attribute("href").Value;
                    addresses.Add(new Listing { ListingAddress = hrefs[0].Value });

                }

            }

            return addresses;
        }


        public static class WebPageHelper
        {
            public static string GetWebPage(string url)
            {
                string responseData = "";
                try
                {
                    HttpWebRequest webRequest = default(HttpWebRequest);
                    webRequest = (HttpWebRequest)System.Net.WebRequest.Create(url);
                    StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                    responseData = responseReader.ReadToEnd().Trim();
                    responseReader.Close();
                }
                catch (Exception e)
                {
                    throw;
                }

                return responseData;
            }
            
        }

    }
}