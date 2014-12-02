using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using foreclosures.Exceptions;

namespace foreclosures.Services
{
    public class WebService
    {
        public static string GetWebPage(string url)
        {
            string responseData = "";

            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.AllowAutoRedirect = false;
                webRequest.UserAgent = Constants.USER_AGENT;
                webRequest.Timeout = 10000;


                using (var webResponse = (HttpWebResponse)webRequest.GetResponse())
                {

                    if ((int)webResponse.StatusCode >= 300 && (int)webResponse.StatusCode <= 399)
                    {
                        throw new RedirectedException("web page redirected");
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