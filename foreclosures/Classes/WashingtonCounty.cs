using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace foreclosures.Classes
{
    public class WashingtonCounty : WebPageStrategy
    {
        public string PageUrl { get; set; }
        public List<Listing> addresses { get; set; }

        public int countyId { get; set; }
        public List<Errors> SiteErrors { get; set; }

        public WashingtonCounty(string url)
        {
            this.PageUrl = url;
            this.addresses = new List<Listing>();
            this.SiteErrors = new List<Errors>();
        }







        public List<Listing> ParseAddresses(string pageData)
        {

            int start = pageData.IndexOf("<div id=\"sales\" class=\"editable\">");
            string html = pageData.Substring(start);

            List<Listing> addresses = new List<Listing>();

            string table = html.Trim().Substring(0, html.IndexOf("</div>") + 6).Replace("<hr>", "").Replace("&nbsp;", "");


            XDocument doc = XDocument.Parse(table);

            int i = 0;
            List<XElement> l = doc.Descendants("p").ToList();
            double percent = (100.0 / l.Count()) / 2.0;
            foreach (XElement element in doc.Descendants("p"))
            {
                Globals.tasks[countyId] += percent;

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

            return addresses;
        }

    }
}