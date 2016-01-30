using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using System.Linq;
using System.Timers;
using System;

namespace Rituals.Services
{
    public class RitualsHub : Hub
    {
        static Timer timer;

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
            StartTimer();
        }

        public void StartTimer()
        {
            var countdown = 31;
            timer = new Timer(1000);
            timer.Elapsed += (sender, e) =>
            {
                countdown--;
                UpdateCountdown(countdown);
                if (countdown == 0)
                {
                    timer.Stop();
                    timer.Dispose();
                    var connectedPlayers = GameRoom.GetConnectedPlayers();
                    if (connectedPlayers.All(x => x.StillPlaying))
                    {
                        NextGame();
                        StartTimer();
                    }
                    else
                    {
                        var winnerIds = connectedPlayers
                            .Where(x => !x.StillPlaying)
                            .Select(x => x.ConnectionId)
                            .ToArray();
                        var loserIds = connectedPlayers
                            .Where(x => x.StillPlaying)
                            .Select(p => p.ConnectionId)
                            .ToArray();
                        Clients.Clients(loserIds).endGame(false);
                        loserIds.ToList().ForEach(ci => GameRoom.DropPlayerByConnectionId(ci));
                        if (winnerIds.Length == 1)
                        {
                            Clients.Client(winnerIds.Single()).endGame(true);
                        }
                        else
                        {
                            var gestureId = GameRoom.GetNextGestureId();
                            Clients.Clients(winnerIds).nextGame(winnerIds.Length, gestureId);
                        }
                    }
                }
            };
            timer.Start();
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

        public void UpdateCountdown(int countdown)
        {
            var activePlayers = GameRoom.GetConnectedPlayers();
            Clients.Clients(activePlayers.Select(p => p.ConnectionId).ToArray()).updateCountdown(countdown);
        }

        public void NextGame()
        {
            int gestureId = GameRoom.GetNextGestureId();
            var activePlayers = GameRoom.GetConnectedPlayers();
            Clients
                .Clients(activePlayers.Select(x => x.ConnectionId).ToArray())
                .nextGame(activePlayers.Count, gestureId);
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