using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace foreclosures.Services
{
    public class UrlService
    {
        public static string GetRootLevelUrl(string fullUrl)
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


        public static string GetUrlAbsolutePath(string fullUrl)
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
    }
}