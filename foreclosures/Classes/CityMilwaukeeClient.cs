using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Web;

namespace foreclosures.Classes
{
    public class CityMilwaukeeClient : WebPageStrategy
    {
        public  string PageUrl { get { return "http://city.milwaukee.gov/Current-Listing-12-18-148"; } }
       public List<Listing> addresses { get; set; }

       public County county { get; set; }

       private HttpContext context { get; set; }


        public CityMilwaukeeClient(HttpContext context)
        {

            string month = DateTime.Now.AddMonths(1).Month.ToString();
            this.addresses = new List<Listing>();
            this.context = context;
        }


        public List<Listing> ParseAddresses(string pageData, int ID)
        {

            List<Listing> addresses = new List<Listing>();
            try
            {

               // string[] text = pageData.Split(new string[] { "<div class=\"Freeform CenterZone\">" }, StringSplitOptions.RemoveEmptyEntries);

                string[] tables = pageData.Split(new string[] { "<table border=\"0\" cellpadding=\"1\" cellspacing=\"1\" width=\"100%\">" }, StringSplitOptions.RemoveEmptyEntries);





                string table = tables[2].Trim().Substring(0, tables[2].IndexOf("</tbody>") + 1)
                                                                        .Replace("&nbsp;", "").Replace("<hr />", "").Replace("\t","").Replace("\n","").Replace("\r","")
                                                                        .Replace("diams", "").Replace("<br />", "").Replace("</tbody", "</tbody>");

                XDocument doc = XDocument.Parse(table.Replace("&", "&amp;"));


                double i = 0;
                List<XElement> l = doc.Descendants("tr").ToList();
                double percent = (100.0 / l.Count()) / 2.0;
                foreach (XElement element in doc.Descendants("tr"))
                {
                    TaskLogger.Instance.AddTaskProgress(ID, percent);

                    try
                    {

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

                                XElement ahref = tds[1].Descendants("a").First();
                                XElement img = tds[0].Descendants("img").First();
                                image = baseurl + img.Attribute("src").Value;
                                price = tds[5].Value;

                                addresses.Add(new Listing { ListingAddress = ahref.Value + " Milwaukee, WI", PDFLink = pdf, ScopeOfWork = scope, Image = image, Price = price });

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