using System.Collections.Generic;



namespace foreclosures.Classes
{
   

    public interface WebPageStrategy
    {
   
        string PageUrl { get; set; }
  
        List<Listing> addresses { get; set; }
        int countyId { get; set; }
        List<Listing> ParseAddresses(string pageData);
        List<Errors> SiteErrors { get; set; }
    }


}
