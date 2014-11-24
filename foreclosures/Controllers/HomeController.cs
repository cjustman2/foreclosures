using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.IO;
using foreclosures.Models;
using foreclosures.Utilities;
using foreclosures.Classes;
using System.Threading.Tasks;


namespace foreclosures.Controllers
{
    public class HomeController : Controller
    {

     
        public ActionResult Index()
        {
            List<County> counties;

            using (var db = new ForeclosuresEntities())
            {
                 counties = db.Counties.ToList();
            }

            ViewBag.Counties = new SelectList(counties, "CountyID", "CountyName");


            return View();
        }

        [HttpPost]
        public JsonResult GetAddresses(int countyId)
        {
            var db = new ForeclosuresEntities();
            var county = db.Counties.Find(countyId);
            var removed = db.Listings.Where(x => x.CountyID == countyId && x.BeenRemoved == true).Select(x => new{ListingAddress = x.ListingAddress, isNew = x.IsNew}).ToList();
            var listings = db.Listings.Where(x => x.CountyID == countyId && x.BeenRemoved == false).Select(x => new { Latitude = x.Latitude, Longitude = x.Longitude, ListingAddress = x.ListingAddress, Image = x.Image, IsNew = x.IsNew, PDFLink = x.PDFLink, ScopeOfWork = x.ScopeOfWork, Price = x.Price }).ToList();
            
            return Json(new{List = listings, removedList = removed, County = county.CityCenter });
        }















        [HttpPost]
        public JsonResult PageScrape(int countyId)
        {
            List<Listing> allremoved = new List<Listing>();
            List<Listing> allListingByCounty = new List<Listing>();
            List<Listing> listings = new List<Listing>();
            bool isStarted = false;
            string responseData = "";


            SingletonTaskLogger tasks = SingletonTaskLogger.Instance;
            SingletonErrorLogger errors = SingletonErrorLogger.Instance;

            if (!tasks.ContainsKey(countyId))
            {
                
                isStarted = true;
                tasks.AddNewTask(countyId);

                var context = System.Web.HttpContext.Current;

                Task.Factory.StartNew(() =>
                    {
                        County currentCounty = null;
                        using(var db = new ForeclosuresEntities())
                        {
                            currentCounty = db.Counties.Find(countyId);
                        }


                        WebPageStrategy webPage = WebPageFactory.GetWebPage(countyId, context);
                        webPage.county = currentCounty;

          
               
                try
                {
                    WebPageHelper pageHelper = new WebPageHelper();
                    if (pageHelper.CanCrawlPage(webPage.PageUrl))
                    {
                        responseData = pageHelper.GetWebPage(webPage.PageUrl);
                    }
                    else
                    {
                        errors.AddError(countyId, string.Format("({0}) Can not crawl site", currentCounty.CountyName));
                    }
                }
                catch (Exception ex)
                {
                    string.Format("({0})" + ex.Message, currentCounty.CountyName);
                } 



                if (!string.IsNullOrWhiteSpace(responseData))
                { 

             
                try
                {
                    listings = webPage.ParseAddresses(responseData);
                }
                catch (Exception ex)
                {
                    errors.AddError(countyId, string.Format("({0})" + ex.Message, currentCounty.CountyName));
                 
                }





                Google google = new Google();
                SingletonThrottleAPIHits api = SingletonThrottleAPIHits.Instance;
                api.allowedHitsPerSecond = 4;
                api.seconds = 1;

                if (listings.Count > 0)
                {

                    var db = new ForeclosuresEntities();

                    db.Mark_All_As_Removed(countyId);

                    allListingByCounty = db.Listings.Where(x => x.CountyID == countyId).ToList();

                    double percent = (100.0 / listings.Count) / 2.0;
                    foreach (Listing listing in listings)
                    {

                        tasks.AddTaskProgress(countyId, percent);

                        if (!string.IsNullOrWhiteSpace(listing.ListingAddress))
                        {
                            try
                            {
                                while (!api.AddHit(countyId))
                                {
                                    System.Threading.Thread.Sleep(200);
                                }


                                google.GeoCodeAddress(listing);
                            }
                            catch (WebException we)
                            {
                                errors.AddError(countyId, string.Format("({0})" + we.Message, currentCounty.CountyName));
                            }
                            catch (Exception ex)
                            {
                                errors.AddError(countyId, string.Format("({0})" + ex.Message, currentCounty.CountyName));
                            }




                            try
                            {

                                listing.ModifiedDate = DateTime.Now;
                                listing.CountyID = countyId;
                                Listing exists = null;
                                exists = allListingByCounty.FirstOrDefault(x => x.ListingAddress.ToLower().Replace(" ", "") == listing.ListingAddress.ToLower().Replace(" ", ""));
                                if (exists != null)
                                {
                                   
                                    exists.Latitude = listing.Latitude;
                                    exists.Longitude = listing.Longitude;
                                    exists.Image = listing.Image;
                                    exists.PDFLink = listing.PDFLink;
                                    exists.ListingAddress = listing.ListingAddress;
                                    exists.ScopeOfWork = listing.ScopeOfWork;
                                    exists.Price = listing.Price;
                                    exists.ModifiedDate = DateTime.Now;
                                    exists.BeenRemoved = false;
                                    db.Listings.Attach(exists);
                                    db.Entry(exists).State = System.Data.Entity.EntityState.Modified;


                                }
                                else
                                {
                                    listing.IsNew = true;
                                    listing.BeenRemoved = false;
                                    db.Listings.Add(listing);

                                }


                                db.SaveChanges();
                                exists = null;

                            }
                            catch (Exception ex)
                            {
                                errors.AddError(countyId, string.Format("({0})" + ex.Message, currentCounty.CountyName));
                            }

                        }

                    }



                    allremoved = db.Listings.Where(x => x.BeenRemoved == true && x.CountyID == countyId).ToList();

                    foreach (Listing listing in allremoved)
                    {
                        try
                        {
                            listing.IsNew = false;
                            db.Listings.Attach(listing);
                            db.Entry(listing).State = System.Data.Entity.EntityState.Modified;

                            db.SaveChanges();
                        }
                        catch (System.Data.Entity.Validation.DbEntityValidationException dbex)
                        {
                            errors.AddError(countyId, string.Format("({0})"+ dbex.Message, currentCounty.CountyName));
                        }
                    }

                    db.Database.Connection.Close();


                }
                else
                {
                    errors.AddError(countyId, string.Format("({0}) No Listings Found", currentCounty.CountyName));
                }

                }

          
              tasks.DeleteTask(countyId);

                    });
                
              
            }

            
            return Json(new { IsStarted = isStarted}, JsonRequestBehavior.DenyGet);
        }



        public ActionResult Progress(int countyId)
        {
            
            double percent = SingletonTaskLogger.Instance.ContainsKey(countyId) ? SingletonTaskLogger.Instance.GetTaskProgress(countyId) : 100;
            List<string> errors = new List<string>();
            errors = SingletonErrorLogger.Instance.GetErrorsById(countyId);

            if (errors.Count > 0)
            {
                string i = "";
            }
          
            return Json(new { Complete = percent, Errors = errors }, JsonRequestBehavior.DenyGet);
        }
    }
}