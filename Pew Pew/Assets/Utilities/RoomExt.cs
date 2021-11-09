using Photon.Pun;
using Photon.Realtime;

namespace Utilities
{
    public static class RoomExt
    {

        #region Settings

        public static void SetOneShot(this Room room, bool b)
        {
            room.SetPropertyValue(RoomProperties.OneShot, b);
        }

        public static bool GetOneShot(this Room room)
        {
            return room.GetPropertyValue(RoomProperties.OneShot, false);
        }

        // TDM points to win
        public static void SetTDMPointsToWin(this Room room, int amount)
        {
            room.SetPropertyValue(RoomProperties.TDM_PointsToWin, amount);
        }

        public static int GetTDMPointsToWin(this Room room)
        {
            return room.GetPropertyValue(RoomProperties.TDM_PointsToWin, 1000);
        }

        // TDM points for kill
        public static void SetTDMPointsKill(this Room room, int amount)
        {
            room.SetPropertyValue(RoomProperties.TDM_PointsKill, amount);
        }

        public static int GetTDMPointsKill(this Room room)
        {
            return room.GetPropertyValue(RoomProperties.TDM_PointsKill, 30);
        }

        // TDM points for headshot kill
        public static void SetTDMPointsHeadshot(this Room room, int amount)
        {
            room.SetPropertyValue(RoomProperties.TDM_PointsHeadshot, amount);
        }

        public static int GetTDMPointsHeadshot(this Room room)
        {
            return room.GetPropertyValue(RoomProperties.TDM_PointsHeadshot, 50);
        }

        // Team Points - Red
        public static void SetTDMRedPoints(this Room room, int amount)
        {
            room.SetPropertyValue(RoomProperties.TDM_RedTeamScore, amount);
        }

        public static int GetTDMRedPoints(this Room room)
        {
            return room.GetPropertyValue(RoomProperties.TDM_RedTeamScore, 50);
        }

        public static void AddTDMRedPoints(this Room room, int amount)
        {
            room.AddValueToProperty(RoomProperties.TDM_RedTeamScore, amount);
        }

        // Team Points - Blue
        public static void SetTDMBluePoints(this Room room, int amount)
        {
            room.SetPropertyValue(RoomProperties.TDM_BlueTeamScore, amount);
        }

        public static int GetTDMBluePoints(this Room room)
        {
            return room.GetPropertyValue(RoomProperties.TDM_BlueTeamScore, 50);
        }

        public static void AddTDMBluePoints(this Room room, int amount)
        {
            room.AddValueToProperty(RoomProperties.TDM_BlueTeamScore, amount);
        }

        #endregion
    }
}