using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace foreclosures.Controllers
{
    public class AdminController : Controller
    {

        [HttpGet]
        public JsonResult LocationHierarchy()
        {
            Dictionary<string, Dictionary<string, string>> hierarchy = new Dictionary<string, Dictionary<string, string>>();
            var db = new ForeclosuresEntities();
            List<County> counties = db.Counties.ToList();

            foreach (County county in counties)
            {
                List<CountyListingType> attsList = db.CountyListingTypes.Where(x => x.typeId == county.CountyID).OrderBy(x => x.displayOrder).ToList();

                Dictionary<string, string> atts = new Dictionary<string, string>();

                foreach (CountyListingType type in attsList)
                {

                    atts.Add(type.typeId.ToString(), type.typeName);
                }

                hierarchy.Add(county.CountyName, atts);
            }

            db.Database.Connection.Close();
            db.Dispose();

            return Json(new { Data = hierarchy }, JsonRequestBehavior.AllowGet);
        }
	}
}