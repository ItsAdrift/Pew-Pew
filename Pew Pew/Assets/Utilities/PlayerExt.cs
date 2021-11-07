using Photon.Pun;
using Photon.Realtime;

namespace Utilities
{
    public static class PlayerExt
    {

        #region Score

        public static void SetKills(this Player player, int amount)
        {
            player.SetPropertyValue(PlayerProperties.Kills, amount);
        }

        public static int GetKills(this Player player)
        {
            return player.GetPropertyValue(PlayerProperties.Kills, 0);
        }

        public static void AddKills(this Player player, int amount)
        {
            player.AddValueToProperty(PlayerProperties.Kills, amount);
        }

        public static void SetDeaths(this Player player, int amount)
        {
            player.SetPropertyValue(PlayerProperties.Deaths, amount);
        }

        public static int GetDeaths(this Player player)
        {
            return player.GetPropertyValue(PlayerProperties.Deaths, 0);
        }

        public static void AddDeaths(this Player player, int amount)
        {
            player.AddValueToProperty(PlayerProperties.Deaths, amount);
        }

        #endregion

        #region Team
        public static int GetTeam(this Player player)
        {
            return player.GetPropertyValue(PlayerProperties.Team, -1);
        }

        public static void SetTeam(this Player player, int teamIdx)
        {
            player.SetPropertyValue(PlayerProperties.Team, teamIdx);
        }

        // Checks if another player is friendly towards our local player
        public static bool IsFriendly(this Player player)
        {
            var ownTeam = PhotonNetwork.LocalPlayer.GetTeam();
            var targetTeam = player.GetTeam();
            return ownTeam == targetTeam;
        }
        #endregion
    }
}