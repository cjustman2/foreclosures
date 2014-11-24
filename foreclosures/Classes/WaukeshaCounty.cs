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
        public County county { get; set; }


        public System.Web.HttpContext currentContext { get; set; }
      

        public WaukeshaCounty(string url, System.Web.HttpContext context)
        {
            this.PageUrl = url;
            this.addresses = new List<Listing>();
            this.currentContext = context;
        }






        public List<Listing> ParseAddresses(string pageData)
        {

            try
            {

                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();

               
                htmlDoc.OptionFixNestedTags = true;


                htmlDoc.LoadHtml(pageData); 

             

                int total = htmlDoc.DocumentNode.SelectNodes("//div[@id='ctl00_MainContent_ContentBlock1']/p/a").ToList().Count;
                double percent = (100.0 / total) / 2.0;
                int i = 0;
                foreach (HtmlNode link in htmlDoc.DocumentNode.SelectNodes("//div[@id='ctl00_MainContent_ContentBlock1']/p/a"))
                {
                    SingletonTaskLogger.Instance.AddTaskProgress(county.CountyID, percent);

                    if (i > 25)
                    {

                        string file = "http://www.waukeshacounty.gov" + link.Attributes["href"].Value;

                        int last = link.Attributes["href"].Value.LastIndexOf('/');
                        string fileName = link.Attributes["href"].Value.Substring(last);
                        string[] fileParts = fileName.Split('.');
                        string extension = fileParts[1];
                        string name = fileParts[0];

                        string filePath = currentContext.Server.MapPath("/Downloads" + fileName);



                       
                        using (WebClient client = new WebClient())
                        {

                            try
                            {
                                client.DownloadFile(file, filePath);
                            
                            }
                            catch (WebException we)
                            {
                                SingletonErrorLogger.Instance.AddError(county.CountyID, string.Format("({0})" + we.Message, county.CountyName));
                            }


                        }

                        try
                        {
                            string address = null;
                            Utilities.Utilities util = new Utilities.Utilities();
                            address = util.ReadPdfFile(filePath);



                            if (string.IsNullOrWhiteSpace(address))
                            {

                                string imagePath = currentContext.Server.MapPath("/PdfToImages" + name + ".png");

                                util.PdfToImage(filePath, imagePath);

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
                        catch(Exception ex)
                        {
                            SingletonErrorLogger.Instance.AddError(county.CountyID, string.Format("({0})" + ex.Message, county.CountyName));
                        }
                        



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
                            SingletonErrorLogger.Instance.AddError(county.CountyID, string.Format("({0}) Address not found.", county.CountyName));
                        }
                        break;

                    }
                }
            }
            catch 
            {
                throw;
            }

            return code;


        }
    }
}