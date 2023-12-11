using CommandSystem;
using InventorySystem.Items;
using InventorySystem;
using MapGeneration;
using MEC;
using Mirror;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Core.Zones;
using Respawning;
using SCPSLAudioApi.AudioCore;
using slocLoader.AutoObjectLoader;
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
using slocLoader;
using Axwabo.Helpers.Config;

namespace ZetaWarhead.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class test3 : CommandSystem.ICommand
    {

        public string Command { get; } = "testtp";

        public string[] Aliases { get; } = new string[] { "ttp" };

        public string Description { get; } = "test (nie używaj jeżeli nie wiesz co to robi)";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            /*AutomaticObjectLoader.TryGetObjects(arguments.At(0), out var s);
            response = "Lista zespawnowanych obiektów o nazwie " + arguments.At(0)+":";
            foreach(var z in s)
            {
                response += $"Typ: {z.Type} | InstanceId: {z.InstanceId} | Pozycja: {z.Transform.Position.x},{z.Transform.Position.y},{z.Transform.Position.z}";
            }
            if (bool.Parse(arguments.At(1)))
            {*/
                //var zetawarhead1object = s.ElementAt(Convert.ToInt32(arguments.At(2)));
                // var room = Axwabo.Helpers.Config.ConfigHelper.GetRoomByType(Axwabo.Helpers.Config.RoomType.EzEndoof);
                //var point = new PositionByRoomName("ZetaWActivate", new MapPointByRoomType(RoomType.EzEndoof, Vector3.up, new SerializedRotation(0, 30)));
                var point = new PositionByRoomName("ZetaWActivate", new MapPointByName("EZ_Endoof", new SerializedRotation(0.005f - 0.0556f, 0.499f + 0.63f, -8.032f + 0.3800001f), new SerializedRotation(10)));


                var itempos = point.Location().WorldPose().position;
                if (bool.Parse(arguments.At(0)) == true)
                {
                    if (!InventoryItemLoader.AvailableItems.TryGetValue(ItemType.KeycardJanitor, out ItemBase ButtonItem))
                    {
                        response = "ok";
                        return false;
                    }

                    var button = NightStars_Plugin.API.Extensions.CreatePickup(itempos, ButtonItem);
                    button.GetComponent<Rigidbody>().isKinematic = true;
                    button.name = "ZetaWarheadActivationButton1";
                    NetworkServer.Spawn(button.gameObject);
                }
                Player.Get(sender).Position = itempos;
            response = "\n\nok👍";
            return true;
        }
    }
}
