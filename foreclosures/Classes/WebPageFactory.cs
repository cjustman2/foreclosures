using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace foreclosures.Classes
{
    public class WebPageFactory
    {

        public static WebPageStrategy Instance(int ID, System.Web.HttpContext context)
        {
            WebPageStrategy webpage = null;
        


            switch (ID)
            {
                case 1:
                    webpage = new CityMilwaukeeClient(context);
                    break;
                case 5:
                    webpage = new WashingtonCounty(context);
                    break;
                case 1003:
                    webpage = new WaukeshaCounty(context); 
                   break;
                case 4:
                  // webpage = new MilwaukeeSheriffSales();
                   break;
                case 2:
                    webpage = new CityMilwaukeeExtended(context);
                    break;
                case 1005:
                    webpage = new DodgeCounty(context);
                    break;
                case 1006:
                    webpage = new SawyerCounty(context);
                    break;
                default:
                    break;
            }



            return webpage;
        }
    }
}