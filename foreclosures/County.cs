//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace foreclosures
{
    using System;
    using System.Collections.Generic;
    
    public partial class County
    {
        public County()
        {
            this.Listings = new HashSet<Listing>();
        }
    
        public int CountyID { get; set; }
        public string CountyName { get; set; }
        public string SiteAddress { get; set; }
        public string CityCenter { get; set; }
    
        public virtual ICollection<Listing> Listings { get; set; }
    }
}