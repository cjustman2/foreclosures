using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Net;
using System.IO;

namespace foreclosures.Utilities
{
    public class WebPageHelper
    {
        const string USER_AGENT = "BrewTownSoftwareLLC_Bot";

        public string GetRootLevelUrl(string fullUrl)
        {
            string rootLevelUrl = null;
            try
            {

                Uri uri;


                Uri.TryCreate(fullUrl, UriKind.Absolute, out uri);
                
                    rootLevelUrl = uri.GetLeftPart(UriPartial.Authority);
                
            }
            catch
            {
                throw;
            }

            return "http://" + rootLevelUrl;
        }

        public bool CanCrawlPage(string pageToCrawl)
        {
             
          

           List<string> disallowedUrls = new List<string>();
           List<string> agents = new List<string>();
           string rootLevelUrl = null;
           bool canCrawl = true;
           string robotTxt = null;


           try
           {
               rootLevelUrl = GetRootLevelUrl(pageToCrawl);
         

               try
               {
                   try
                   {
                       robotTxt = GetWebPage(rootLevelUrl + "/robots.txt");
                   }
                   catch (FileNotFoundException fe) 
                   {
                       return true;
                   }
                   catch (Exception ex)
                   {
                       return false;
                   }




                   List<string> robotsList = System.Text.RegularExpressions.Regex.Split(robotTxt, @"(?=User-agent:)").Where(x => x != string.Empty).ToList();

                   foreach (string robot in robotsList)
                   {
                       List<string> entries = robot.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                       List<string> useragent = entries.Where(x => x.Contains("User-agent")).ToList();
                       List<string> disallows = entries.Where(x => x.Contains("Disallow")).ToList();

                       foreach (string agent in useragent)
                       {
                           int start = agent.IndexOf(":") + 1;

                           string user = agent.Substring(start);

                           if (user.Trim() == "*" || user.Trim() == USER_AGENT)
                           {

                               foreach (string disallow in disallows)
                               {
                                   int starting = disallow.IndexOf(":") + 1;

                                   string notallowed = disallow.Substring(starting);

                                   disallowedUrls.Add(notallowed.Trim());
                               }
                           }
                       }

                   }

         

                   foreach (string file in disallowedUrls)
                   {
                  
                       if(pageToCrawl.ToLower().Contains(file))
                       {
                           canCrawl = false;
                       }
                   }





               }
               catch (Exception ex)
               {

               }


           }
           catch (Exception ex)
           {

           }




            return canCrawl;
        }



        public string GetWebPage(string url)
        {
            string responseData = "";

            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.AllowAutoRedirect = false;
                webRequest.UserAgent = USER_AGENT;
                webRequest.Timeout = 10000;


                using (var webResponse = (HttpWebResponse)webRequest.GetResponse())
                {

                    if ((int)webResponse.StatusCode >= 300 && (int)webResponse.StatusCode <= 399)
                    {

                       // string uriString = webResponse.Headers["Location"];

                        throw new FileNotFoundException("File was not Fount.");
                    }
                    else
                    {
                        using (Stream stream = webResponse.GetResponseStream())
                        {
                            StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                            responseData = reader.ReadToEnd();
                        }
                    }

                }

            }
            catch
            {
                throw;
            }


            return responseData;
        }
    }



}