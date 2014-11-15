using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Xml.Linq;
using foreclosures.Models;

namespace foreclosures.Utilities
{
    public class Google
    {

        public Listing GeoCodeAddress(Listing listing)
        {
           if(!string.IsNullOrWhiteSpace(listing.ListingAddress)){

            var requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false", Uri.EscapeDataString(listing.ListingAddress));

            var request = WebRequest.Create(requestUri);
            var response = request.GetResponse();
            if (response != null) 
            { 
            var xdoc = XDocument.Load(response.GetResponseStream());

            var result = xdoc.Element("GeocodeResponse").Element("result");
            if (result != null)
            {
                var locationElement = result.Element("geometry").Element("location");
                listing.Latitude = locationElement.Element("lat").Value.ToString();
                listing.Longitude = locationElement.Element("lng").Value.ToString();
            }
        }

           }

            return listing;
        }
    }
}