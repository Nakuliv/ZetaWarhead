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
    public class test2 : CommandSystem.ICommand
    {

        public string Command { get; } = "testnetidslist";

        public string[] Aliases { get; } = new string[] { "tnetidsls" };

        public string Description { get; } = "test (nie używaj jeżeli nie wiesz co to robi)";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            /*response = $"Wszystkie pokoje z NetId:";
            foreach (var room in RoomIdentifier.AllRoomIdentifiers.Where(x => x.gameObject.GetComponent<NetworkIdentity>() != null))
            {
                response += $"\n- {room.Name}";
            }*/
            response = $"Lista obiektów z netid z All w nazwie:";
            var _networkIdentities = UnityEngine.GameObject.FindObjectsOfType<NetworkIdentity>().Where(x => x.name.Contains("All")).ToList();
            foreach (var n in _networkIdentities)
            {
                response += $"\n- {n.name} | {n.netId} | {n.assetId}";
            }
            response += "\n\nok👍";
            if (bool.Parse(arguments.At(0)) == true) Log.Debug(response);
            return true;
        }
    }
}
