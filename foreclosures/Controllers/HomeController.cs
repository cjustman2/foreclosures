using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.IO;
using foreclosures;
using foreclosures.Models;
using foreclosures.Classes;
using foreclosures.Services;
using foreclosures.Utilities;
using System.Threading.Tasks;
using System.Reflection;


namespace foreclosures.Controllers
{
    public class HomeController : Controller
    {


        public ActionResult Index()
        {
           

         


            return View();
        }

        [HttpPost]
        public JsonResult GetAddressesByCounty(int countyId)
        {
            var db = new ForeclosuresEntities();
            var county = db.Counties.Find(countyId);
            var removed = db.Listings.Where(x => x.CountyID == countyId && x.BeenRemoved == true).Select(x => new { ListingAddress = x.ListingAddress, isNew = x.IsNew }).ToList();
            var listings = db.Listings.Where(x => x.CountyID == countyId && x.BeenRemoved == false).Select(x => new { Latitude = x.Latitude, Longitude = x.Longitude, ListingAddress = x.ListingAddress, Image = x.Image, IsNew = x.IsNew, PDFLink = x.PDFLink, ScopeOfWork = x.ScopeOfWork, Price = x.Price, attributeName = x.Attribute.attributeName, attributeId = x.Attribute.attributeId == null ? 0 :x.Attribute.attributeId }).ToList();

            db.Database.Connection.Close();
            db.Dispose();
            return Json(new { List = listings, removedList = removed, County = county.CityCenter });
        }

        [HttpPost]
        public JsonResult GetAddressesByCity(int cityId)
        {
            string cityCenter = "";
            var db = new ForeclosuresEntities();
          
            var removed = db.Listings.Where(x => x.cityId == cityId && x.BeenRemoved == true).Select(x => new { ListingAddress = x.ListingAddress, isNew = x.IsNew }).ToList();
            var listings = db.Listings.Where(x => x.cityId == cityId && x.BeenRemoved == false).Select(x => new { Latitude = x.Latitude, Longitude = x.Longitude, ListingAddress = x.ListingAddress, Image = x.Image, IsNew = x.IsNew, PDFLink = x.PDFLink, ScopeOfWork = x.ScopeOfWork, Price = x.Price, center = x.County.CityCenter }).ToList();

             db.Database.Connection.Close();
            db.Dispose();

            cityCenter = listings.Count > 0 ? listings[0].center : "Wisconsin";

            return Json(new { List = listings, removedList = removed, County = cityCenter });
        }


        [HttpPost]
        public JsonResult GetAddressesByAttribute(int ID)
        {
            string cityCenter = "";
            var db = new ForeclosuresEntities();

          
                var removed = db.Listings.Where(x => x.attributeId == ID && x.BeenRemoved == true).Select(x => new { ListingAddress = x.ListingAddress, isNew = x.IsNew }).ToList();
                var listings = db.Listings.Where(x => x.attributeId == ID && x.BeenRemoved == false).Select(x => new { Latitude = x.Latitude, Longitude = x.Longitude, ListingAddress = x.ListingAddress, Image = x.Image, IsNew = x.IsNew, PDFLink = x.PDFLink, ScopeOfWork = x.ScopeOfWork, Price = x.Price, center = x.County.CityCenter }).ToList();
            
            db.Database.Connection.Close();
            db.Dispose();

            cityCenter = listings.Count > 0 ? listings[0].center : "Wisconsin";
            return Json(new { List = listings, removedList = removed, County = cityCenter });
        }










        [HttpPost]
        public JsonResult PageScrape(int ID)
        {
            List<Listing> allremoved = new List<Listing>();
            List<Listing> allListings = new List<Listing>();
            List<Listing> listings = new List<Listing>();
            bool isStarted = false;
            string responseData = "";


            TaskLogger tasks = TaskLogger.Instance;
            ErrorLogger errors = ErrorLogger.Instance;

           


            if (!tasks.ContainsKey(ID))
            {

                isStarted = true;
                tasks.AddNewTask(ID);

                var context = System.Web.HttpContext.Current;

                Task.Factory.StartNew(() =>
                    {
                        County currentCounty = null;
                        using (var db = new ForeclosuresEntities())
                        {
                            Attribute a = db.Attributes.Find(ID);
                            currentCounty = db.Counties.Find(a.typeId);
                            
                        }

                        WebPageStrategy webPage = WebPageFactory.Instance(ID, context);
                        webPage.county = currentCounty;



                        try
                        {

                            RobotDotTxtService pageHelper = new RobotDotTxtService();
                            if (pageHelper.CanCrawlPage(webPage.PageUrl))
                            {
                                responseData =WebService.GetWebPage(webPage.PageUrl);
                            }
                            else
                            {
                                errors.AddError(ID, string.Format("({0}) Can not crawl site", currentCounty.CountyName));
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
                                listings = webPage.ParseAddresses(responseData,ID);
                            }
                            catch (Exception ex)
                            {
                                errors.AddError(ID, string.Format("({0})" + ex.Message, currentCounty.CountyName));

                            }



                            

                            GoogleService google = new GoogleService();
                            ThrottleRequestService api = ThrottleRequestService.Instance;
                            api.allowedHitsPerSecond = 4;
                            api.seconds = 1;

                            if (listings.Count > 0 && errors.GetErrorsById(ID).Count == 0)
                            {

                                var db = new ForeclosuresEntities();

                                db.Mark_All_As_Removed(ID);

                                allListings = db.Listings.Where(x => x.attributeId == ID).ToList();

                                double percent = (100.0 / listings.Count) / 2.0;


                                foreach (Listing listing in listings)
                                {

                                    tasks.AddTaskProgress(ID, percent);

                                    if (!string.IsNullOrWhiteSpace(listing.ListingAddress))
                                    {





                                        try
                                        {

                                            listing.zipcode = AddressService.GetZipCodeFromEndOfAddress(listing.ListingAddress);

                                         
                                               
                                               string cityName = AddressService.GetCityNameFromAddress(listing.ListingAddress);

                                                if (!string.IsNullOrWhiteSpace(cityName))
                                                {
                                                    using (var cont = new ForeclosuresEntities())
                                                    {
                                                       
                                                       City city = cont.Cities.Where(x => x.cityName.ToLower() == cityName.ToLower()).FirstOrDefault();

                                                        if (city == null)
                                                        {
                                                            city = new City();
                                                            city.countyId = currentCounty.CountyID;
                                                            city.cityName = cityName;
                                                            cont.Cities.Add(city);
                                                            cont.SaveChanges();
                                                        }

                                                        listing.cityId = city.cityId;
                                                    }
                                                }
                                            
                                        }
                                        catch (Exception ex)
                                        {
                                            errors.AddError(ID, string.Format("({0})" + ex.Message, currentCounty.CountyName));
                                        }









                                        try
                                        {

                                            while (!api.AddHit(ID))
                                            {
                                                System.Threading.Thread.Sleep(200);
                                            }


                                            google.GeoCodeAddress(listing);
                                        }
                                        catch (WebException we)
                                        {
                                            errors.AddError(ID, string.Format("({0})" + we.Message, currentCounty.CountyName));
                                        }
                                        catch (Exception ex)
                                        {
                                            errors.AddError(ID, string.Format("({0})" + ex.Message, currentCounty.CountyName));
                                        }









                                        try
                                        {

                                            listing.ModifiedDate = DateTime.Now;
                                            listing.attributeId = ID;
                                            listing.CountyID = currentCounty.CountyID;

                                            Listing exists = null;
                                            exists = allListings.FirstOrDefault(x => x.ListingAddress.ToLower().Replace(" ", "") == listing.ListingAddress.ToLower().Replace(" ", ""));
                                            if (exists != null)
                                            {



                                                exists.zipcode = listing.zipcode;
                                                exists.cityId = listing.cityId;
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
                                            errors.AddError(ID, string.Format("({0})" + ex.Message, currentCounty.CountyName));
                                        }

                                    }

                                }







                                allremoved = db.Listings.Where(x => x.BeenRemoved == true && x.CountyID == ID).ToList();

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
                                        errors.AddError(ID, string.Format("({0})" + dbex.Message, currentCounty.CountyName));
                                    }
                                }

                                db.Database.Connection.Close();


                            }
                            else
                            {
                                errors.AddError(ID, string.Format("({0}) No Listings Found", currentCounty.CountyName));
                            }

                        }


                        tasks.DeleteTask(ID);

                    });


            }


            return Json(new { IsStarted = isStarted }, JsonRequestBehavior.DenyGet);
        }



        public ActionResult Progress(int countyId)
        {

            double percent = TaskLogger.Instance.ContainsKey(countyId) ? TaskLogger.Instance.GetTaskProgress(countyId) : 100;
            List<string> errors = new List<string>();
            errors = ErrorLogger.Instance.GetErrorsById(countyId);

            if (errors.Count > 0)
            {
                string i = "";
            }

            return Json(new { Complete = percent, Errors = errors }, JsonRequestBehavior.DenyGet);
        }
    }
}