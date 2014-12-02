using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace foreclosures.Classes
{
    public class TaskLogger
    {
        private Dictionary<int, double> tasks{get;set;}
   
        private static volatile TaskLogger instance;
        private static object syncRoot = new Object();
        private TaskLogger()
        {
            this.tasks = new Dictionary<int, double>();
        }
           

        public static TaskLogger Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new TaskLogger();
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

    public class ErrorLogger
    {
    
        private Dictionary<int, List<string>> errors { get; set; }
        private static volatile ErrorLogger instance;
        private static object syncRoot = new Object();
        private ErrorLogger()
        {
           
            this.errors = new Dictionary<int, List<string>>();
        }

        public static ErrorLogger Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new ErrorLogger();
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
