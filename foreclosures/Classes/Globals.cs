using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace foreclosures.Classes
{
    public static class Globals
    {

        public static Dictionary<int, double> tasks = new Dictionary<int, double>();
    }


    public class Errors
    {
        public string originator { get; set; }
        public Exception exception { get; set; }

        public Errors() {
            this.exception = new Exception();
        }
    }
}
