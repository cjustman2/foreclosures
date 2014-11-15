using System.Collections.Generic;
using HtmlAgilityPack;

namespace foreclosures.Classes
{
    public class SawyerCounty : WebPageStrategy
    {
        public string PageUrl { get; set; }
       public  List<Listing> addresses { get; set; }

       public int countyId { get; set; }
       public List<Errors> SiteErrors { get; set; }
        public SawyerCounty(string url)
        {
            this.PageUrl = url;
            this.addresses = new List<Listing>();
            this.SiteErrors = new List<Errors>();
        }


        public List<Listing> ParseAddresses(string pageData)
        {
            List<Listing> addresses = new List<Listing>();

            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();

            // There are various options, set as needed
            htmlDoc.OptionFixNestedTags = true;

            // filePath is a path to a file containing the html
            // htmlDoc.Load(pageData);

            htmlDoc.LoadHtml(pageData);


            double percent = (100 / htmlDoc.DocumentNode.SelectNodes("//td[@class='DetailsCol c_ForeclosureSale']").Count) / 2;
            double i = 0;
            foreach (HtmlNode text in htmlDoc.DocumentNode.SelectNodes("//td[@class='DetailsCol c_ForeclosureSale']"))
            {
                Globals.tasks[countyId] += percent;

                if (!string.IsNullOrWhiteSpace(text.InnerHtml) && text.InnerHtml != "&nbsp;")
                {
                    text.InnerHtml = text.InnerHtml.Replace("Property address:", "Property:").Replace("Property addresses:", "Property:");


                    string ad = text.InnerHtml.Replace("<br>", "").Substring(text.InnerHtml.IndexOf("Property:") + 1);
                    string address = ad.Substring(0, ad.IndexOf("Attorney:")).Replace("(BOTH PARCELS SOLD TOGETHER PER ATTORNEY)", "");

                    if (!string.IsNullOrWhiteSpace(address))
                    {
                        Listing item = new Listing();
                        item.ListingAddress = address;
                        addresses.Add(item);
                    }
                }

                i++;
            }

           
            return addresses;
        }
    }
}