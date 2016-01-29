using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace Rituals.Services
{
    public class RitualsHub : Hub
    {
        public void Send(string name, string message)
        {
            Clients.All.addNewMessageToPage(name, message);
        }

        public void Hello()
        {
            Clients.All.hello();
        }
    }
}