
namespace PlayFab.Networking
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Mirror;
    using UnityEngine.Events;
    using Sirenix.OdinInspector;

    [Serializable]
    public class PlayfabConnection
    {
        public bool IsAuthenticated;
        public string PlayFabId;
        public string LobbyId;
        public int ConnectionId;
        public NetworkConnection Connection;
    }

    public class CustomGameServerMessageTypes
    {
        public const short ReceiveAuthenticate = 900;
        public const short ShutdownMessage = 901;
        public const short MaintenanceMessage = 902;
    }

    public class ReceiveAuthenticateMessage : NetworkMessage
    {
        public string PlayFabId;
    }

    public class ShutdownMessage : NetworkMessage { }

    // todo: Upgrade https://github.com/vis2k/Mirror/pull/2317
    // [Serializable]
    public class MaintenanceMessage : NetworkMessage
    {
    //     public DateTime ScheduledMaintenanceUTC;

    //     public override void Deserialize(NetworkReader reader)
    //     {
    //         var json = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);
    //         ScheduledMaintenanceUTC = json.DeserializeObject<DateTime>(reader.ReadString());
    //     }

    //     public override void Serialize(NetworkWriter writer)
    //     {
    //         var json = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);
    //         var str = json.SerializeObject(ScheduledMaintenanceUTC);
    //         writer.WriteString(str);
    //     }
    }
}