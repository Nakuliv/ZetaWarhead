using CommandSystem;
using MapGeneration;
using MEC;
using Mirror;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Core.Zones;
using Respawning;
using SCPSLAudioApi.AudioCore;
using Subtitles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UnityEngine;
using Utils.Networking;

namespace ZetaWarhead.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class test : CommandSystem.ICommand
    {

        public string Command { get; } = "testnetidbring";

        public string[] Aliases { get; } = new string[] { "tnetidb" };

        public string Description { get; } = "test (nie używaj jeżeli nie wiesz co to robi)";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            /*response = $"Wszystkie pokoje z NetId:";
            foreach (var room in RoomIdentifier.AllRoomIdentifiers.Where(x => x.gameObject.GetComponent<NetworkIdentity>() != null))
            {
                response += $"\n- {room.Name}";
            }*/
            var _networkIdentities = UnityEngine.GameObject.FindObjectsOfType<NetworkIdentity>().Where(x => x.name.Contains("All")).ToList();
            var room = _networkIdentities.FirstOrDefault(x => x?.assetId == Convert.ToUInt32(arguments.At(0)));
            if(arguments.At(1) == "unspawn" || arguments.At(1) == "move")NetworkServer.UnSpawn(room.gameObject);
            if(arguments.At(1) == "destroy")NetworkServer.Destroy(room.gameObject);
            room.transform.position = Player.Get(sender).Position;
            if(arguments.At(1) == "spawn" || arguments.At(1) == "move") NetworkServer.Spawn(room.gameObject);
            response = "\n\nok👍";
            return true;
        }
    }
}
