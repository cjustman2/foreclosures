using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Web;

namespace foreclosures.Classes
{
    public class CityMilwaukeeExtended : WebPageStrategy
    {
        public  string PageUrl { get { return "http://city.milwaukee.gov/ExtendedListing"; } }
        public List<Listing> addresses { get; set; }
     
        public County county { get; set; }
        private HttpContext context { get; set; }

        public CityMilwaukeeExtended( HttpContext context)
        {
         
            this.addresses = new List<Listing>();
            this.context = context;
        }






        public List<Listing> ParseAddresses(string pageData, int ID)
        {
            List<Listing> addresses = new List<Listing>();

            try
            {

              //  string[] text = pageData.Split(new string[] { "<div class=\"Freeform CenterZone\">" }, StringSplitOptions.RemoveEmptyEntries);

                string[] tables = pageData.Split(new string[] { "<table border=\"0\" cellpadding=\"1\" cellspacing=\"1\" width=\"100%\">" }, StringSplitOptions.RemoveEmptyEntries);





                string table = tables[2].Trim().Substring(0, tables[2].IndexOf("</tbody>") + 1).Replace("&nbsp;", "").Replace("</tbody", "</tbody>").Replace("</tbody>>", "</tbody>");

                XDocument doc = XDocument.Parse(table.Replace("&", "&amp;"));

                int i = 0;
                List<XElement> l = doc.Descendants("tr").ToList();
                double percent = (100.0 / l.Count()) / 2.0;
                foreach (XElement element in doc.Descendants("tr"))
                {
                    try
                    {

                        TaskLogger.Instance.AddTaskProgress(ID, percent);
                        if (i > 0)
                        {

                            List<XElement> tds = element.Descendants("td").ToList();
                            if (tds.Count > 1)
                            {
                                string baseurl = "http://city.milwaukee.gov";
                                string pdf = "", scope = "", image = "", price = "";



                                List<XElement> hrefs = tds[1].Descendants("a").ToList();
                                if (hrefs.Count > 1)
                                {
                                    pdf = baseurl + hrefs[0].Attribute("href").Value;
                                    scope = baseurl + hrefs[1].Attribute("href").Value;
                                }



                                price = tds[5].Value;
                                image = baseurl + tds[0].Descendants("img").First().Attribute("src").Value;

                                addresses.Add(new Listing { ListingAddress = hrefs[0].Value + " Milwaukee, WI", PDFLink = pdf, ScopeOfWork = scope, Image = image, Price = price });

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.Instance.AddError(county.CountyID, string.Format("({0})" + ex.Message, county.CountyName));
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