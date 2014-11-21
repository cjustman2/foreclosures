using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using HtmlAgilityPack;
using foreclosures.Utilities;
using System.Net;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using tessnet2;
using Ghostscript;
using GhostscriptSharp;
using GhostscriptSharp.Settings;
using System.Text.RegularExpressions;
using System.IO;
using foreclosures.Controllers;

namespace foreclosures.Classes
{

    public class WaukeshaCounty : WebPageStrategy
    {
        public string PageUrl { get; set; }
   
        public Listing listing = null;
        public  List<Listing> addresses { get; set; }
        public List<Errors> SiteErrors { get; set; }
        public int countyId { get; set; }

        public System.Web.HttpContext currentContext { get; set; }
      

        public WaukeshaCounty(string url, System.Web.HttpContext context)
        {
            this.PageUrl = url;
            this.SiteErrors = new List<Errors>();
            this.addresses = new List<Listing>();
            this.currentContext = context;
        }






        public List<Listing> ParseAddresses(string pageData)
        {



            

            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();

            // There are various options, set as needed
            htmlDoc.OptionFixNestedTags = true;

            // filePath is a path to a file containing the html
            // htmlDoc.Load(pageData);

            htmlDoc.LoadHtml(pageData); // to load from a string (was htmlDoc.LoadXML(xmlString)

            // ParseErrors is an ArrayList containing any errors from the Load statement
            if (htmlDoc.ParseErrors != null && htmlDoc.ParseErrors.Count() > 0)
            {
                // Handle any parse errors as required

            }
            else
            {
               int total = htmlDoc.DocumentNode.SelectNodes("//div[@id='ctl00_MainContent_ContentBlock1']/p/a").ToList().Count;
                double percent = (100.0 / total) / 2.0;
                int i = 0;
                foreach (HtmlNode link in htmlDoc.DocumentNode.SelectNodes("//div[@id='ctl00_MainContent_ContentBlock1']/p/a"))
                {
                    Globals.tasks[countyId] += percent;
                    if (i > 25)
                    {
                    
                        string file = "http://www.waukeshacounty.gov" + link.Attributes["href"].Value;

                        int last = link.Attributes["href"].Value.LastIndexOf('/');
                        string fileName = link.Attributes["href"].Value.Substring(last);
                        string[] fileParts = fileName.Split('.');
                        string extension = fileParts[1];
                        string name = fileParts[0];

                        string filePath = currentContext.Server.MapPath("/Downloads" + fileName);



                        bool isDownloaded = false;
                        using (WebClient client = new WebClient())
                        {

                            try
                            {
                                client.DownloadFile(file, filePath);
                                isDownloaded = true;
                            }
                            catch (Exception ex)
                            {
                                Classes.Errors e = new Errors() { exception = ex, originator = file};
                                SiteErrors.Add(e);
                            }


                        }

                        if (isDownloaded)
                        {
                            string address = null;
                            Utilities.Utilities util = new Utilities.Utilities();
                            address = util.ReadPdfFile(filePath);



                            if (string.IsNullOrWhiteSpace(address))
                            {

                                string imagePath = currentContext.Server.MapPath("/PdfToImages" + name + ".png");

                                util.GetPdfThumbnail(filePath, imagePath);

                                address = ReadImage(imagePath);
                            }



                            if (!string.IsNullOrWhiteSpace(address))
                            {

                            listing = new Listing()
                            {

                                Image = "",
                                ListingAddress = address,
                                PDFLink = "/Downloads" + fileName,
                                ScopeOfWork = ""
                            };

                            
                                addresses.Add(listing);
                            }



                        }
                            
                    

                    }



                    i++;
                }

            }
            return addresses;
        }







        public string ReadImage(string imagePath)
        {
            string code = "";

            try
            {

                var image = new Bitmap(imagePath);
                tessnet2.Tesseract ocr = new tessnet2.Tesseract();
                //ocr.SetVariable("tessedit_char_whitelist", "0123456789abcdefghijklmnopqrstuvwxyz");
                ocr.Init(@"C:\\Users\\user\\Documents\\Visual Studio 2013\\Projects\\foreclosures\\tessdata", "eng", false);
                List<tessnet2.Word> result = ocr.DoOCR(image, new System.Drawing.Rectangle());

              
                Regex regex = new Regex(@"^?\d{5}(?:[-\s]\d{4})?");
                int j;

                for (int i = 0; i < result.Count; i++)
                {
                    if (result[i].Text.ToLower().Contains("address"))
                    {

                        j = i + 1;
                        do
                        {
                            code += result[j].Text + " ";
                            j++;
                        } while (!regex.IsMatch(result[j].Text));

                        code += result[j].Text.Replace("of property: ", "");
                        if (string.IsNullOrWhiteSpace(code))
                        {
                            Classes.Errors e = new Errors() { exception = new Exception() { } };
                            SiteErrors.Add(e);
                        }
                        break;

                    }
                }
            }
            catch (Exception exception)
            {
                Classes.Errors e = new Errors();
                e.exception = exception;
                e.originator = imagePath;
                SiteErrors.Add(e);
            }

            return code;


        }
    }
}