using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Web;
using System;
namespace foreclosures.Classes
{
    public class WashingtonCounty : WebPageStrategy
    {
        public string PageUrl { get; set; }
        public List<Listing> addresses { get; set; }

        public County county { get; set; }
        private HttpContext context { get; set; }

        public WashingtonCounty(string url,HttpContext context)
        {
            this.PageUrl = url;
            this.addresses = new List<Listing>();
            this.context = context;
        }







        public List<Listing> ParseAddresses(string pageData)
        {
            List<Listing> addresses = new List<Listing>();
       
            try
            {

                int start = pageData.IndexOf("<div id=\"sales\" class=\"editable\">");
                string html = pageData.Substring(start);



                string table = html.Trim().Substring(0, html.IndexOf("</div>") + 6).Replace("<hr>", "").Replace("&nbsp;", "");


                XDocument doc = XDocument.Parse(table);

                int i = 0;
                List<XElement> l = doc.Descendants("p").ToList();
                double percent = (100.0 / l.Count()) / 2.0;
                foreach (XElement element in doc.Descendants("p"))
                {
                    SingletonTaskLogger.Instance.AddTaskProgress(county.CountyID, percent);

                    try
                    {
                        List<XElement> hrefs = element.Descendants("a").ToList();
                        if (hrefs.Count > 0)
                        {
                            string file = "http://www.washingtoncountysheriffwi.org"; ;

                            if (hrefs[0].Value.ToLower().Replace(" ", "") != "backtotop")
                            {
                                file += hrefs[0].Attribute("href").Value;
                                addresses.Add(new Listing { ListingAddress = hrefs[0].Value });
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        SingletonErrorLogger.Instance.AddError(county.CountyID, string.Format("({0})" + ex.Message, county.CountyName));
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