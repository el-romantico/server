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
                UpdateLoser();
                UpdateWinners();
            }
            UpdateUI();
        }

        private void UpdateWinners()
        {
            int winnersCount = GameRoom.GetSuccessfulCount();
            if (winnersCount == 1)
            {
                var winner = GameRoom.GetWinner();
                this.Clients.Client(winner.ConnectionId).endGame(true);
            }
            else
            {
                int nextGameGesture = 1;
                this.Clients.All.nextGame(winnersCount, nextGameGesture);
            }
            GameRoom.RestartPlayersState();
        }

        private void UpdateLoser()
        {
            var loser = GameRoom.GetLoser();
            GameRoom.DropPlayerByConnectionId(loser.ConnectionId);
            var loserClient = this.Clients.Client(loser.ConnectionId);
            loserClient.endGame(false);
            loserClient.Stop();
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