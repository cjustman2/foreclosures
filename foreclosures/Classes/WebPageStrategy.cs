using System.Collections.Generic;



namespace foreclosures.Classes
{
   

    public interface WebPageStrategy
    {
   
        string PageUrl { get; }
  
        List<Listing> addresses { get; set; }
        County county { get; set; }
        List<Listing> ParseAddresses(string pageData, int ID);
     
    }


}
