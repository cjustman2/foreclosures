using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace foreclosures.Classes
{
    public class pageScraperHub : Hub
    {
        private List<string> observers { get; set; }
        public override System.Threading.Tasks.Task OnConnected()
        {
            
            var name = Context.User.Identity;
            return base.OnConnected();
        }
        public void Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(name, message);
        }
    }
}