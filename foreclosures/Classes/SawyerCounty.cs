using System.Collections.Generic;
using HtmlAgilityPack;
using System;
using System.Web;

namespace foreclosures.Classes
{
    public class SawyerCounty : WebPageStrategy
    {
        public string PageUrl { get; set; }
       public  List<Listing> addresses { get; set; }
       private HttpContext context { get; set; }
       public County county { get; set; }

       public SawyerCounty(string url, HttpContext context)
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
                    SingletonTaskLogger.Instance.AddTaskProgress(county.CountyID, percent);

                    try
                    {
                        if (!string.IsNullOrWhiteSpace(text.InnerHtml) && text.InnerHtml != "&nbsp;")
                        {
                            text.InnerHtml = text.InnerHtml.Replace("Property address:", "Property:").Replace("Property addresses:", "Property:");


                            string ad = text.InnerHtml.Replace("<br>", "").Substring(text.InnerHtml.IndexOf("Property:") + 1);
                            string address = ad.Substring(0, ad.IndexOf("Attorney:")).Replace("(BOTH PARCELS SOLD TOGETHER PER ATTORNEY)", "").Replace("54817Parcel #00893833 5210", "54817");

                            if (!string.IsNullOrWhiteSpace(address))
                            {
                                Listing item = new Listing();
                                item.ListingAddress = address;
                                addresses.Add(item);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        SingletonErrorLogger.Instance.AddError(county.CountyID, string.Format("({0})" + ex.Message, county.CountyName));

                    }

                    i++;
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