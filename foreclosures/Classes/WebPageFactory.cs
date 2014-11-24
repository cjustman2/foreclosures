using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace foreclosures.Classes
{
    public class WebPageFactory
    {

        public static WebPageStrategy GetWebPage(int ID, System.Web.HttpContext context)
        {
            WebPageStrategy webpage = null;
            string url = null;

            using (var db = new ForeclosuresEntities())
            {
                url = db.Counties.Where(x => x.CountyID == ID).Select(x => x.SiteAddress).First();

            }


            switch (ID)
            {
                case 1:
                    webpage = new CityMilwaukeeClient(url,context);
                    break;
                case 2:
                    webpage = new WashingtonCounty(url,context);
                    break;
                case 3:
                    webpage = new WaukeshaCounty(url,context);
                 
                    break;
                case 4:
                    webpage = new CityMilwaukeeExtended(url,context);
                    break;
                case 5:
                    webpage = new DodgeCounty(url,context);
                    break;
                case 6:
                    webpage = new SawyerCounty(url,context);
                    break;
                default:
                    break;
            }



            return webpage;
        }
    }
}