using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using System.Linq;
using System.Timers;

namespace Rituals.Services
{
    public class RitualsHub : Hub
    {
        private static object theLock = 42;

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
            EndGame(GameRoom.GetConnectedPlayers().Select(x => x.ConnectionId).ToArray(), false);
            GameRoom.DropAllPlayers();
            UpdateUI();
        }

        public void TimeoutExpired()
        {
            lock(theLock)
            {
                var expiredForEveryone = GameRoom.TimeoutOutExpiredForConnectionId(Context.ConnectionId);
                if (expiredForEveryone)
                {
                    TimeoutExpiredForEveryOne();
                }
            }
        }

        private void TimeoutExpiredForEveryOne()
        {
            var winnerIds = GameRoom.GetWinnersConnectionIds();
            var loserIds = GameRoom.GetLosersConnectionIds();
            bool thereAreActivePlayers = winnerIds.Length + loserIds.Length > 0;
            if (thereAreActivePlayers)
            {
                if(winnerIds.Length == 0)
                {
                    NextGame();
                }
                else
                {
                    EndGame(loserIds, false);
                    if (winnerIds.Length == 1)
                    {
                        EndGame(winnerIds, true);
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
                EndGame(new string[] { winner.ConnectionId }, true);
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
            EndGame(new string[] { loser.ConnectionId }, false);
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

        private void EndGame(string[] connectionIds, bool outcome)
        {
            for (int i = 0; i < connectionIds.Length; i++)
            {
                GameRoom.DropPlayerByConnectionId(connectionIds[i]);
            }
            Clients.Clients(connectionIds).endGame(outcome);
            UpdateUI();
        }
    }
}