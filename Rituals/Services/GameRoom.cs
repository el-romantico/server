using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rituals.Services
{
    public static class GameRoom
    {
        private static List<Player> allPlayers = new List<Player>();

        public static void AddPlayer(string connectionId)
        {
            allPlayers.Add(new Player() { ConnectionId = connectionId, StillPlaying = true });
        }

        public static void DropPlayerByConnectionId(string connectionId)
        {
            allPlayers.RemoveAt(allPlayers.FindIndex(p => p.ConnectionId == connectionId));
        }

        internal static void PlayerSuccess(string connectionId)
        {
            allPlayers[allPlayers.FindIndex(x => x.ConnectionId == connectionId)].StillPlaying = false;
        }

        internal static bool CheckWinningCondition()
        { 
            return allPlayers.Count(p => p.StillPlaying) == 1;
        }

        internal static Player GetLoser()
        {
            return allPlayers.Single(p => p.StillPlaying);
        }
    }
}