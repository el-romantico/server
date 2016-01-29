using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace Rituals.Services
{
    public class RitualsHub : Hub
    {
        public void CheckConnection()
        {
            Clients.All.checkConnection();
        }

        public void Connect()
        {
            GameRoom.AddPlayer(Context.ConnectionId);
            UpdateUI();
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
            if (winningCondition)
            {
                var loser = GameRoom.GetLoser();
                GameRoom.DropPlayerByConnectionId(loser.ConnectionId);
                this.Clients.Client(loser.ConnectionId).Stop();
                int nextGameGesture = 1;
                this.Clients.All.nextGame(nextGameGesture);
            }
            UpdateUI();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            GameRoom.DropPlayerByConnectionId(Context.ConnectionId);
            this.Clients.Client(Context.ConnectionId).Stop();
            UpdateUI();
            return base.OnDisconnected(stopCalled);
        }

        private void UpdateUI()
        {
            Clients.All.connectedCount(GameRoom.GetConnectedCount());
            Clients.All.successfulCount(GameRoom.GetSuccessfulCount());
        }
    }
}