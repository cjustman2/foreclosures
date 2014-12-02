using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace foreclosures.Exceptions
{
    public class RedirectedException : Exception
    {
        public RedirectedException(string message) : base(message) { }
    }
}