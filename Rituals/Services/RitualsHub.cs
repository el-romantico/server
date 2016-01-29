using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public void Connect()
        {
            GameRoom.AddPlayer(Context.ConnectionId);
            // Show real time on web site
        }

        public void StartGame()
        {
            int gestureNumber = 0; // Random
            Clients.All.startGame(gestureNumber);
        }

        public void Success()
        {
            GameRoom.PlayerSuccess(Context.ConnectionId);
            var winningCondition = GameRoom.CheckWinningCondition();
            if(winningCondition)
            {
                var loser = GameRoom.GetLoser();
                GameRoom.DropPlayerByConnectionId(loser.ConnectionId);
                this.Clients.Client(loser.ConnectionId).Stop();
                int nextGameGesture = 1;
                this.Clients.All.nextGame(nextGameGesture);
            }
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            GameRoom.DropPlayerByConnectionId(Context.ConnectionId);
            this.Clients.Client(Context.ConnectionId).Stop();
            return base.OnDisconnected(stopCalled);
        }
    }
}