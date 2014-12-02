using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace foreclosures.Controllers
{
    public class DashboardController : Controller
    {
        public JsonResult GetCities(int countyId)
       {
           var db = new ForeclosuresEntities();
           var cities = db.Cities.Where(x => x.countyId == countyId).OrderBy(x => x.cityName).ToList();

           db.Database.Connection.Close();
           db.Dispose();

            

           return Json(new {Cities = cities }, JsonRequestBehavior.DenyGet);
       }

        public JsonResult GetCounties()
        {
            

            var db = new ForeclosuresEntities();

            var counties = db.Counties.Select(x => new{ CountyName = x.CountyName, CountyID = x.CountyID}).ToList();

                db.Database.Connection.Close();
                db.Dispose();


            return Json(new { Counties = counties}, JsonRequestBehavior.DenyGet);
        }


        public JsonResult GetAttributes(int ID)
        {
            var db = new ForeclosuresEntities();

            var attributes = db.Attributes.Where(x => x.typeId == ID).Select(x => new{ AttributeID = x.attributeId, AttributeName = x.attributeName, x.displayOrder}).OrderBy(x => x.displayOrder).ToList();

            db.Database.Connection.Close();
            db.Dispose();


            return Json(new { Attributes = attributes }, JsonRequestBehavior.DenyGet);
        }
     
	}
}