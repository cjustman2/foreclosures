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
    
    public partial class Attribute
    {
        public Attribute()
        {
            this.Listings = new HashSet<Listing>();
        }
    
        public int attributeId { get; set; }
        public string attributeName { get; set; }
        public Nullable<int> typeId { get; set; }
        public Nullable<int> displayOrder { get; set; }
    
        public virtual ICollection<Listing> Listings { get; set; }
    }
}
