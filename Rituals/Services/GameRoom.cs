using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web;

namespace Rituals.Services
{
    public static class GameRoom
    {
        private static List<Player> allPlayers = new List<Player>();

        public static void AddPlayer(string connectionId)
        {
            if(!allPlayers.Any(x => x.ConnectionId == connectionId))
                allPlayers.Add(new Player() { ConnectionId = connectionId, StillPlaying = true });
        }

        internal static void DropAllPlayers()
        {
            allPlayers.Clear();
        }
        
        public static void DropPlayerByConnectionId(string connectionId)
        {
            if (allPlayers.Any(x => x.ConnectionId == connectionId))
                allPlayers.RemoveAt(allPlayers.FindIndex(p => p.ConnectionId == connectionId));
        }

        internal static int GetConnectedCount()
        {
            return allPlayers.Count;
        }

        internal static List<Player> GetConnectedPlayers()
        {
            return allPlayers.ToList();
        }

        internal static void PlayerSuccess(string connectionId)
        {
            allPlayers[allPlayers.FindIndex(x => x.ConnectionId == connectionId)].StillPlaying = false;
        }

        internal static void RestartPlayersState()
        {
            allPlayers.ForEach(p => p.StillPlaying = true);
        }

        internal static bool CheckWinningCondition()
        { 
            return allPlayers.Count(p => p.StillPlaying) == 1;
        }

        internal static Player GetLoser()
        {
            return allPlayers.Single(p => p.StillPlaying);
        }

        internal static Player GetWinner()
        {
            return allPlayers.Single(p => !p.StillPlaying);
        }

        internal static int GetSuccessfulCount()
        {
            return allPlayers.Count(x => !x.StillPlaying);
        }

        internal static int GetNextGestureId()
        {
            return 5;
        }
    }
}