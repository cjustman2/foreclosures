using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System;
using System.Web;

namespace foreclosures.Classes
{
    public class DodgeCounty : WebPageStrategy
    {
        public string PageUrl { get; set; }
       public List<Listing> addresses { get; set; }
       private HttpContext context { get; set; }
       public County county { get; set; }
       public DodgeCounty(string url, HttpContext context)
        {
            this.PageUrl = url;
            this.addresses = new List<Listing>();
            this.context = context;
        }







        public List<Listing> ParseAddresses(string pageData)
        {

            try
            {

                int start = pageData.IndexOf("<div id=\"ctl00_content_Screen\">");
                string html = pageData.Substring(start);



                string table = html.Trim().Substring(0, html.IndexOf("</div>") + 6).Replace("<hr>", "").Replace("&nbsp;", "");


                XDocument doc = XDocument.Parse(table);
                int total = 0;
                foreach (XElement element in doc.Descendants("ul"))
                {
                    List<XElement> asss = element.Descendants("a").ToList();
                    total += asss.Count;
                }
                double percent = (100.0 / total) / 2.0;

                foreach (XElement element in doc.Descendants("ul"))
                {
                    List<XElement> lis = element.Descendants("li").ToList();



                    for (int i = 0; i < lis.Count; i++)
                    {

                        try
                        {

                            List<XElement> hrefs = lis[i].Descendants("a").ToList();

                            if (hrefs.Count > 0)
                            {

                                SingletonTaskLogger.Instance.AddTaskProgress(county.CountyID, percent);

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
                                address.PDFLink = file;
                                addresses.Add(address);
                            }
                        }
                        catch (Exception ex)
                        {
                            SingletonErrorLogger.Instance.AddError(county.CountyID, string.Format("({0})" + ex.Message, county.CountyName));
                        }

                    }



                }
            }
            catch
            {
                throw;
            }


            return addresses;
        }


    }

}