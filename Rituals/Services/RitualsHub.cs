using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using System.Linq;
using System.Timers;

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
            int gestureNumber = GameRoom.GetNextGestureId();
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

        public void DisconnectAll()
        {
            GameRoom.DropAllPlayers();
            UpdateUI();
        }
        
        public void TimeoutExpired()
        {
            var expiredForEveryone = GameRoom.TimeoutOutExpiredForConnectionId(Context.ConnectionId);
            if(expiredForEveryone)
            {
                TimeoutExpiredForEveryOne();
            }
        }

        private void TimeoutExpiredForEveryOne()
        {
            var connectedPlayers = GameRoom.GetConnectedPlayers();
            if (connectedPlayers.Any())
            {
                if (connectedPlayers.All(x => x.StillPlaying))
                {
                    NextGame();
                }
                else
                {
                    var winnerIds = GameRoom.GetWinnersConnectionIds();
                    var loserIds = GameRoom.GetLosersConnectionIds();
                    Clients.Clients(loserIds).endGame(false);
                    loserIds.ToList().ForEach(ci => GameRoom.DropPlayerByConnectionId(ci));
                    if (winnerIds.Length == 1)
                    {
                        Clients.Client(winnerIds.Single()).endGame(true);
                    }
                    else
                    {
                        NextGame();
                    }
                }
            }
        }

        public void UpdateUI()
        {
            Clients.All.connectedCount(GameRoom.GetConnectedCount());
            Clients.All.successfulCount(GameRoom.GetSuccessfulCount());
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            GameRoom.DropPlayerByConnectionId(Context.ConnectionId);
            this.Clients.Client(Context.ConnectionId).Stop();
            UpdateUI();
            return base.OnDisconnected(stopCalled);
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
                NextGame();
            }
            GameRoom.RestartPlayersState();
        }

        private void UpdateLoser()
        {
            var loser = GameRoom.GetLoser();
            GameRoom.DropPlayerByConnectionId(loser.ConnectionId);
            var loserClient = this.Clients.Client(loser.ConnectionId);
            loserClient.endGame(false);
        }
        
        private void NextGame()
        {
            GameRoom.RestartPlayersState();
            int gestureId = GameRoom.GetNextGestureId();
            var activePlayers = GameRoom.GetConnectedPlayers();
            Clients
                .Clients(activePlayers.Select(x => x.ConnectionId).ToArray())
                .nextGame(activePlayers.Count, gestureId);
            UpdateUI();
        }
    }
}