using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace foreclosures.Services
{
    public class AddressService
    {
      private static List<string> states = new List<string>()    {"AL","AK","AZ","AR","CA","CO","CT","DE","FL","GA","HI","ID","IL","IN","IA","KS","KY","LA","ME","MD","MA","MI","MN","MS",
"MO","MT","NE","NV","NH","NJ","NM","NY","NC","ND","OH","OK","OR","PA","RI","SC","SD","TN","TX","UT","VT","VA","WA","WV","WI","WY"};

            public static float GetZipCodeFromEndOfAddress(string address)
            {
                float ZipCode = 0;

                try
                {
                   
                   
                    string cleanedAddress = address.Replace(",", "");
                    string[] addressWords = cleanedAddress.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                    Regex regex = new Regex(@"^?\d{5}(?:[-\s]\d{4})?");

                    if (regex.IsMatch(addressWords[addressWords.Length - 1]))
                    {
                        string zip = addressWords[addressWords.Length - 1];

                        int i = zip.IndexOf('-');
                        if (i > 0)
                        {
                            ZipCode = float.Parse(zip.Substring(0, i));
                        }
                        else
                        {
                            ZipCode = float.Parse(zip);
                        }
                    }

              

                }
                catch
                {
                    throw;
                }

                return ZipCode;


            }


            public static string GetCityNameFromAddress(string address)
            {
               
                string city = null;
                try
                {
                    string cleanedAddress = address.Replace(",", "");
                    string[] addressWords = cleanedAddress.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);





                    Regex regex_ = new Regex(@"^[a-zA-Z]+(?:[\s-][a-zA-Z]+)*$");
                    for (int i = 0; i < addressWords.Length - 1; i++)
                    {
                        if (!states.Contains(addressWords[(addressWords.Length - 1) - i].ToUpper()) && regex_.IsMatch(addressWords[(addressWords.Length - 1) - i]))
                        {
                            city = addressWords[(addressWords.Length - 1) - i];
                            break;
                        }

                    }

                }
                catch 
                {
                    throw;
                }


                return city;
            }

        
    }
}