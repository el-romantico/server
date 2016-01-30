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
            int activePlayersCount = GameRoom.GetConnectedCount();
            int gestureNumber = 0; // Random
            Clients.All.startGame(activePlayersCount, gestureNumber);
        }

        public void Success()
        {
            GameRoom.PlayerSuccess(Context.ConnectionId);
            var winningCondition = GameRoom.CheckWinningCondition();
            if (winningCondition)
            {
                var loser = GameRoom.GetLoser();
                GameRoom.DropPlayerByConnectionId(loser.ConnectionId);
                var looserClient = this.Clients.Client(loser.ConnectionId);
                looserClient.endGame(false);
                looserClient.Stop();
                int nextGameGesture = 1;
                int activePlayersCount = GameRoom.GetConnectedCount() - GameRoom.GetSuccessfulCount();
                if(activePlayersCount == 1)
                {
                    var winner = GameRoom.GetWinner();
                    this.Clients.Client(winner.ConnectionId).endGame(true);
                }
                else
                {
                    this.Clients.All.nextGame(activePlayersCount, nextGameGesture);
                }
            }
            UpdateUI();
        }

        public void UpdateUI()
        {
            Clients.All.connectedCount(GameRoom.GetConnectedCount());
            Clients.All.successfulCount(GameRoom.GetSuccessfulCount());
        }

        public void DisconnectAll()
        {
            GameRoom.DropAllPlayers();
            UpdateUI();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            GameRoom.DropPlayerByConnectionId(Context.ConnectionId);
            this.Clients.Client(Context.ConnectionId).Stop();
            UpdateUI();
            return base.OnDisconnected(stopCalled);
        }
    }
}