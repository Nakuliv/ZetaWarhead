using CommandSystem;
using Mirror;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ZetaWarhead.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ForeceDeconReverse : CommandSystem.ICommand
    {
        public string Command { get; } = "ForceDeconReverse";

        public string[] Aliases { get; } = new string[] { "ForceDR" };

        public string Description { get; } = "Odwraca proces dekontaminacji lighta";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {

            ZetaWarheadController.ReverseDecon();
            response = $"Odwrócono proces dekontaminacji lighta!";
            return true;
        }
    }
}
