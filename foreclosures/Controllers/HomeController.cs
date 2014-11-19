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
         
            bool isStarted = false;
            if (!Globals.tasks.ContainsKey(countyId))
            {
                
                isStarted = true;

                Globals.tasks.Add(countyId, 0);
                var context = System.Web.HttpContext.Current;

                Task.Factory.StartNew(() =>
                    {


                        WebPageStrategy webPage = WebPageFactory.GetWebPage(countyId, context);
                      
                webPage.countyId = countyId;

                List<Listing> allremoved = new List<Listing>();
                List<Listing> allListingByCounty = new List<Listing>();
                string responseData = "";
                try
                {
                   responseData = PageParser.WebPageHelper.GetWebPage(webPage.PageUrl);
                }
                catch (Exception ex)
                {
                    webPage.SiteErrors.Add(new Errors(){originator = webPage.PageUrl, exception = new Exception(){}});
                }

                if (!string.IsNullOrWhiteSpace(responseData) && webPage.SiteErrors.Count == 0)
                { 


                List<Listing> listings = new List<Listing>();

                bool hasErrors = false;
                try
                {
                    listings = webPage.ParseAddresses(responseData);
                }
                catch (Exception ex)
                {
                    hasErrors = true;
                }

                Google google = new Google();

                if (listings.Count > 0 && !hasErrors)
                {

                    var db = new ForeclosuresEntities();

                    db.Mark_All_As_Removed(countyId);

                    allListingByCounty = db.Listings.Where(x => x.CountyID == countyId).ToList();

                    double i = (100.0 / listings.Count) / 2.0;
                    foreach (Listing listing in listings)
                    {
                        Globals.tasks[countyId] = Globals.tasks[countyId] + i;


                        if (!string.IsNullOrWhiteSpace(listing.ListingAddress))
                        {
                            try
                            {
                                google.GeoCodeAddress(listing);
                            }
                            catch (Exception ex) 
                            {

                            }

                            if (!string.IsNullOrWhiteSpace(listing.Latitude))
                            {

                                listing.ModifiedDate = DateTime.Now;
                                listing.CountyID = countyId;
                                Listing exists = null;
                                exists = allListingByCounty.FirstOrDefault(x => x.ListingAddress.ToLower().Replace(" ", "") == listing.ListingAddress.ToLower().Replace(" ", ""));
                                if (exists != null)
                                {

                                    exists.IsNew = false;
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

                            System.Threading.Thread.Sleep(150);
                        }

                    }



                    allremoved = db.Listings.Where(x => x.BeenRemoved == true && x.CountyID == countyId).ToList();

                    foreach (Listing listing in allremoved)
                    {
                        listing.IsNew = false;
                        db.Listings.Attach(listing);
                        db.Entry(listing).State = System.Data.Entity.EntityState.Modified;

                        db.SaveChanges();
                    }

                    db.Database.Connection.Close();


                }
                }

              Globals.tasks.Remove(countyId);

                    });

              
            }

            
            return Json(new { IsStarted = isStarted}, JsonRequestBehavior.DenyGet);
        }



        public ActionResult Progress(int countyId)
        {
            return Json(Globals.tasks.Keys.Contains(countyId) ? Globals.tasks[countyId] : 100, JsonRequestBehavior.DenyGet);
        }
    }
}