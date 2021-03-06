﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace foreclosures.Utilities
{
    public class ThrottleRequestService
    {

                    public int hits { get; private set; }
            public int allowedHitsPerSecond { get; set; }
            public int seconds{get; set;}
            private DateTime currentSecond = DateTime.Now;
            public List<string> logger = new List<string>();

    private static volatile ThrottleRequestService instance;
   private static object syncRoot = new Object();

   private ThrottleRequestService() { hits = 1; }

   public static ThrottleRequestService Instance
   {
      get
      {
         if (instance == null)
         {
            lock (syncRoot)
            {
               if (instance == null)
                  instance = new ThrottleRequestService();
            }
         }

         return instance;
      }
   }



            public bool AddHit(int thread)
            {
                lock (syncRoot)
                {

                    if (hits <= allowedHitsPerSecond)
                    {
                        logger.Add(thread + ": " + currentSecond);
                        hits++;
                        return true;
                    }
                    else
                    {
                        if (DateTime.Now > currentSecond.AddSeconds(seconds))
                        {
                            
                            currentSecond = DateTime.Now;
                            logger.Add(thread + ": " + currentSecond);
                            hits = 1;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
    }
}