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

            return rootLevelUrl;
        }


        public string GetUrlAbsolutePath(string fullUrl)
        {
            string absolutePath = null;
            try
            {

                Uri uri;


                Uri.TryCreate(fullUrl, UriKind.Absolute, out uri);

                absolutePath = uri.AbsolutePath;

            }
            catch
            {
                throw;
            }

            return absolutePath;
        }
        public bool CanCrawlPage(string pageToCrawl)
        {

            

            List<string> allowedUrls = new List<string>();
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
                   catch (WebException we)
                   {
                       var resp = (HttpWebResponse)we.Response;
                       if (resp.StatusCode == HttpStatusCode.NotFound)
                       {

                           return true;

                       }
                       else
                       {

                           return false;
                       }
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
                       List<string> allows = entries.Where(x => x.Contains("Allow")).ToList();

                       foreach (string allow in allows)
                       {
                           int starting = allow.IndexOf(":") + 1;

                           allowedUrls.Add(allow.Substring(starting).Trim());
                       }



                       foreach (string agent in useragent)
                       {
                           int start = agent.IndexOf(":") + 1;

                           string user = agent.Substring(start);

                           if (user.Trim() == "*" || user.Trim() == USER_AGENT)
                           {

                               foreach (string disallow in disallows)
                               {
                                   int starting = disallow.IndexOf(":") + 1;

                                   disallowedUrls.Add(disallow.Substring(starting).Trim());
                               }
                           }
                       }

                   }


                   string absolute = GetUrlAbsolutePath(pageToCrawl);
                   if(!allowedUrls.Contains("/"))
                   {

                           foreach (string file in disallowedUrls)
                           {
                  
                               if(pageToCrawl.ToLower().Contains(file) && !allowedUrls.Contains(absolute))
                               {
                                   canCrawl = false;
                               }
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

                        throw new FileNotFoundException("You are being redirected.");
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