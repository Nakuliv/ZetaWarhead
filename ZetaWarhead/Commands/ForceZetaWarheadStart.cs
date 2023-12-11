using CommandSystem;
using MEC;
using Mirror;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Core.Zones;
using PluginAPI.Core.Zones.Heavy;
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
    public class ForceZetaWarheadStart : CommandSystem.ICommand
    {

        public string Command { get; } = "ForceZetaWarheadStart";

        public string[] Aliases { get; } = new string[] { "ForceZetaWarhead" };

        public string Description { get; } = "Wymusza start Zeta Warheada";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            ZetaWarheadController.Activate();

            response = $"Aktywowano Zeta Warhead!";
            return true;
        }
    }
}
