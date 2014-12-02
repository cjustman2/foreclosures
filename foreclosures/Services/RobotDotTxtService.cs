using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Net;
using System.IO;
using foreclosures.Services;
using foreclosures.Exceptions;

namespace foreclosures.Utilities
{

    
    public class RobotDotTxtService
    {

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
               rootLevelUrl = UrlService.GetRootLevelUrl(pageToCrawl);
         


                   try
                   {
                       robotTxt = WebService.GetWebPage(rootLevelUrl + "/robots.txt");
                   }
                   catch (RedirectedException rd)
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

                   List<string> myList = robotsList.Where(x => x.ToLower().Contains(Constants.USER_AGENT.ToLower())).Count() > 0 ? robotsList.Where(x => x.Contains(Constants.USER_AGENT)).ToList() : robotsList.Where(x => x.Replace(" ", "").Replace("\r", "").ToLower().Contains("user-agent:*")).ToList();

                   foreach (string robotAgent in myList)
                   {
                       List<string> entries = robotAgent.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                       List<string> useragent = entries.Where(x => x.ToLower().Contains("user-agent")).ToList();



                       foreach (string agent in useragent)
                       {

                           List<string> disallows = entries.Where(x => x.ToLower().Contains("disallow")).ToList();
                           List<string> allows = entries.Where(x => x.Contains("Allow")).ToList();
                           foreach (string allow in allows)
                           {
                               int starting = allow.IndexOf(":") + 1;
                               allowedUrls.Add(allow.Substring(starting).Trim());
                           }

                           int start = agent.IndexOf(":") + 1;
                           string user = agent.Substring(start);
                           if (user.Trim() == "*" || user.ToLower().Trim() == Constants.USER_AGENT.ToLower())
                           {
                               foreach (string disallow in disallows)
                               {
                                   int starting = disallow.IndexOf(":") + 1;
                                   disallowedUrls.Add(disallow.Substring(starting).Trim());
                               }
                           }
                       }
                   }





                   string absolute = UrlService.GetUrlAbsolutePath(pageToCrawl);
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
           catch
           {
               throw;
           }




            return canCrawl;
        }




    }



}