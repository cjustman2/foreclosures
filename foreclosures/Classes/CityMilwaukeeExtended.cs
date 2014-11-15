﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace foreclosures.Classes
{
    public class CityMilwaukeeExtended : WebPageStrategy
    {
        public string PageUrl { get; set; }
        public List<Listing> addresses { get; set; }
        public List<Errors> SiteErrors { get; set; }
        public int countyId { get; set; }

        public CityMilwaukeeExtended(string url)
        {
            this.PageUrl = url;
            this.addresses = new List<Listing>();
            this.SiteErrors = new List<Errors>();
        }






        public List<Listing> ParseAddresses(string pageData)
        {

            string[] text = pageData.Split(new string[] { "<div class=\"Freeform CenterZone\">" }, StringSplitOptions.RemoveEmptyEntries);

            string[] tables = text[4].Split(new string[] { "<table border=\"0\" cellspacing=\"1\" cellpadding=\"1\" width=\"100%\">" }, StringSplitOptions.RemoveEmptyEntries);


            List<Listing> addresses = new List<Listing>();


            string table = tables[1].Trim().Substring(0, tables[1].IndexOf("</tbody>") + 1).Replace("&nbsp;", "").Replace("</tbody", "</tbody>");

            XDocument doc = XDocument.Parse(table);

            int i = 0;
            List<XElement> l = doc.Descendants("tr").ToList();
            double percent = (100.0 / l.Count()) / 2.0;
            foreach (XElement element in doc.Descendants("tr"))
            {
                Globals.tasks[countyId] += percent;
                if (i > 0)
                {

                    List<XElement> tds = element.Descendants("td").ToList();
                    if (tds.Count > 1)
                    {
                        string baseurl = "http://city.milwaukee.gov";
                        string pdf = "", scope = "", image = "", price = "";



                        List<XElement> hrefs = tds[0].Descendants("a").ToList();
                        if (hrefs.Count > 1)
                        {
                            pdf = baseurl + hrefs[0].Attribute("href").Value;
                            scope = baseurl + hrefs[1].Attribute("href").Value;
                        }

                        XElement ahref = tds[0].Descendants("a").First();
                  
                        price = tds[4].Value;

                        addresses.Add(new Listing { ListingAddress = ahref.Value + " Milwaukee, WI", PDFLink = pdf, ScopeOfWork = scope, Image = image, Price = price });

                    }
                }

                i++;
            }

            return addresses;
        }

    }
}