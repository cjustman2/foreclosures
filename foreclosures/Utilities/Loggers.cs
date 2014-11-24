using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace foreclosures.Classes
{
    public class SingletonTaskLogger
    {
        private Dictionary<int, double> tasks{get;set;}
   
        private static volatile SingletonTaskLogger instance;
        private static object syncRoot = new Object();
        private SingletonTaskLogger()
        {
            this.tasks = new Dictionary<int, double>();
        }
           

        public static SingletonTaskLogger Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new SingletonTaskLogger();
                    }
                }

                return instance;
            }
        }


 


        public void AddNewTask(int countyId)
        {
            lock (syncRoot)
            {
                this.tasks.Add(countyId, 0.0);
            }
        }

        public void AddTaskProgress(int countyId,double percent)
        {
            lock (syncRoot)
            {
                this.tasks[countyId] += percent;
            }
        }


        public double GetTaskProgress(int countyId)
        {
            lock (syncRoot)
            {
                return this.tasks[countyId];
            }
        }

        public void DeleteTask(int countyId)
        {
            lock (syncRoot)
            {
                this.tasks.Remove(countyId);
            }
        }

        public bool ContainsKey(int id)
        {
            lock (syncRoot)
            {
                return this.tasks.ContainsKey(id);
            }
        }
    }

    public class SingletonErrorLogger
    {
    
        private Dictionary<int, List<string>> errors { get; set; }
        private static volatile SingletonErrorLogger instance;
        private static object syncRoot = new Object();
        private SingletonErrorLogger()
        {
           
            this.errors = new Dictionary<int, List<string>>();
        }

        public static SingletonErrorLogger Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new SingletonErrorLogger();
                    }
                }

                return instance;
            }
        }


        public void AddError(int countyId, string error)
        {
            lock (syncRoot)
            {
                List<string> list;
                if (!this.errors.TryGetValue(countyId, out list))
                    this.errors.Add(countyId, list = new List<string>());


                list.Add(error);
            }

        }


        public List<string> GetErrorsById(int id)
        {
            List<string> list = new List<string>();
            lock (syncRoot)
            {
                if(this.errors.ContainsKey(id))
                list = this.errors[id];

                this.errors.Remove(id);
                
            }
            return list;
        }

        public void DeleteErrorsById(int id)
        {
            lock (syncRoot)
            {
                this.errors.Remove(id);
            }
        }

    }
}
