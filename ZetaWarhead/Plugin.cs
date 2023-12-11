using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items;
using InventorySystem;
using MapGeneration;
using MEC;
using Mirror;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Core.Zones;
using PluginAPI.Core.Zones.Heavy;
using PluginAPI.Core.Zones.Light;
using SCPSLAudioApi.AudioCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using PluginAPI.Core.Zones.Entrance;
using slocLoader.AutoObjectLoader;
using slocLoader;
using Axwabo.Helpers.Config;
using InventorySystem.Items.Pickups;
using PluginAPI.Core.Items;
using PluginAPI.Enums;
using Respawning;
using HarmonyLib;
using ZetaWarhead.Patches;
using PlayerRoles;
using System.Xml.Linq;
using Axwabo.Helpers;
using static RoundSummary;

namespace ZetaWarhead
{
    public class Plugin
    {
        public static Plugin Singleton { get; private set; }

        [PluginEntryPoint("Zeta Warhead for NightStars", "1.0.0", "", "Nakuliv")]
        public void Load()
        {
            Singleton = this;
            var harmony = new Harmony($"zetaWarheadPatch.{DateTime.UtcNow.Ticks}");
            harmony.PatchAll();
            PluginAPI.Events.EventManager.RegisterEvents(this);
            AudioPlayerBase.OnFinishedTrack += OnTrackEnd;
            //API.PrefabsLoaded += OnSlocPrefabsLoaded;
        }

        [PluginEvent(PluginAPI.Enums.ServerEventType.PlayerSearchPickup)]
        private void OnPlySearchPickup(Player ply, ItemPickupBase item)
        {
            if (Round.Duration.TotalMinutes >= 15 && !AlphaWarheadController.InProgress)
            {
                if (item.name == "ZetaWarheadCancelButton")
                {
                    item.DestroySelf();

                    ply.ReceiveHint("====== <color=red>ZETA WARHEAD</color> ======\n" +
                            "Dezaktywowano głowicę zeta.", 5);
                    ZetaWarheadController.Cancel();
                }
                else if (item.name == "ZetaWarheadActivationButton1")
                {
                    if (item.Info.ItemId == ItemType.KeycardO5 && !ZetaWarheadController.isStarted)
                    {
                        ZetaWarheadController.ActivatedButtons--;
                        Timing.CallDelayed(1f, () =>
                        {
                            ZetaWarheadController.SpawnFirstActivateButton();
                        });
                    }
                    else if (item.Info.ItemId == ItemType.KeycardO5 && ZetaWarheadController.isStarted)
                    {
                        ZetaWarheadController.O5_1 = null;
                        return;
                    }
                    else if (ply.CurrentItem is not null && ply.CurrentItem.ItemTypeId == ItemType.KeycardO5)
                    {
                        if (!InventoryItemLoader.AvailableItems.TryGetValue(ItemType.KeycardO5, out ItemBase ButtonItem))
                        {
                            return;
                        }
                        item.DestroySelf();
                        ZetaWarheadController.ActivateBtn_1 = null;
                        var point = new PositionByRoomName("ZetaWActivate", new MapPointByName("EZ_Endoof", new SerializedRotation(0.005f - 0.0556f, 0.499f + 0.61f, -8.032f + 0.3800001f), new SerializedRotation(10)));
                        var itempos = point.Location().WorldPose().position;
                        var button = NightStars_Plugin.API.Extensions.CreatePickup(itempos, ButtonItem);
                        button.GetComponent<Rigidbody>().isKinematic = true;
                        button.name = "ZetaWarheadActivationButton1";
                        ZetaWarheadController.O5_1 = button;
                        NetworkServer.Spawn(button.gameObject);
                        ZetaWarheadController.ActivatedButtons++;
                        ply.RemoveItem(new Item(ply.CurrentItem));
                        if (ZetaWarheadController.ActivatedButtons > 1)
                        {
                            ply.ReceiveHint("====== <color=red>ZETA WARHEAD</color> ======\n" +
                            "Aktywowano wszystkie włączniki\ngłowica zeta została aktywowana.", 5);
                            ZetaWarheadController.Activate();
                        }
                        else if (ZetaWarheadController.ActivatedButtons == 1)
                        {
                            ply.ReceiveHint("====== <color=red>ZETA WARHEAD</color> ======\n" +
                            "Aktywowano włącznik (<color=yellow>1/2</color>)", 5);
                        }
                    }
                    else if (ply.CurrentItem is null || ply.CurrentItem.ItemTypeId != ItemType.KeycardO5)
                    {
                        item.DestroySelf();
                        ZetaWarheadController.ActivateBtn_1 = null;
                        ply.ReceiveHint("====== <color=red>ZETA WARHEAD</color> ======\nMusisz mieć kartę O5 w ręce!", 7);
                        Timing.CallDelayed(1f, () =>
                        {
                            ZetaWarheadController.SpawnFirstActivateButton();
                        });
                    }
                }
                else if (item.name == "ZetaWarheadActivationButton2")
                {
                    if (item.Info.ItemId == ItemType.KeycardO5 && !ZetaWarheadController.isStarted)
                    {
                        ZetaWarheadController.ActivatedButtons--;
                        Timing.CallDelayed(1f, () =>
                        {
                            ZetaWarheadController.SpawnSecondActivateButton();
                        });
                    }
                    else if (item.Info.ItemId == ItemType.KeycardO5 && ZetaWarheadController.isStarted)
                    {
                        ZetaWarheadController.O5_2 = null;
                        return;
                    }
                    else if (ply.CurrentItem is not null && ply.CurrentItem.ItemTypeId == ItemType.KeycardO5)
                    {
                        if (!InventoryItemLoader.AvailableItems.TryGetValue(ItemType.KeycardO5, out ItemBase ButtonItem))
                        {
                            return;
                        }
                        item.DestroySelf();
                        ZetaWarheadController.ActivateBtn_2 = null;
                        var point = new PositionByRoomName("ZetaWActivate", new MapPointByName("EZ_Endoof (1)", new SerializedRotation(0.005f - 0.0556f, 0.499f + 0.61f, -8.032f + 0.3800001f), new SerializedRotation(10)));
                        var itempos = point.Location().WorldPose().position;
                        var button = NightStars_Plugin.API.Extensions.CreatePickup(itempos, ButtonItem);
                        button.GetComponent<Rigidbody>().isKinematic = true;
                        button.name = "ZetaWarheadActivationButton2";
                        ZetaWarheadController.O5_2 = button;
                        NetworkServer.Spawn(button.gameObject);
                        ZetaWarheadController.ActivatedButtons++;
                        ply.RemoveItem(new Item(ply.CurrentItem));
                        if (ZetaWarheadController.ActivatedButtons > 1)
                        {
                            ply.ReceiveHint("====== <color=red>ZETA WARHEAD</color> ======\n" +
                            "Aktywowano wszystkie włączniki\ngłowica zeta została aktywowana.", 5);
                            ZetaWarheadController.Activate();
                        }
                        else if (ZetaWarheadController.ActivatedButtons == 1)
                        {
                            ply.ReceiveHint("====== <color=red>ZETA WARHEAD</color> ======\n" +
                            "Aktywowano włącznik (<color=yellow>1/2</color>)", 5);
                        }
                    }
                    else if (ply.CurrentItem is null || ply.CurrentItem.ItemTypeId != ItemType.KeycardO5)
                    {
                        item.DestroySelf();
                        ZetaWarheadController.ActivateBtn_2 = null;
                        ply.ReceiveHint("====== <color=red>ZETA WARHEAD</color> ======\nMusisz mieć kartę O5 w ręce!", 7);
                        Timing.CallDelayed(1f, () =>
                        {
                            ZetaWarheadController.SpawnSecondActivateButton();
                        });
                    }
                }
            }
            else if (item.name == "ZetaWarheadActivationButton1" && AlphaWarheadController.InProgress)
            {
                item.DestroySelf();
                ZetaWarheadController.ActivateBtn_1 = null;

                ply.ReceiveHint("====== <color=red>ZETA WARHEAD</color> ======\n" +
                         "Zeta Warhead nie może być włączony podczas wybuchu alpha warhead.", 5);
                Timing.CallDelayed(2f, () => ZetaWarheadController.SpawnFirstActivateButton());
            }
            else if (item.name == "ZetaWarheadActivationButton2" && AlphaWarheadController.InProgress)
            {
                item.DestroySelf();
                ZetaWarheadController.ActivateBtn_2 = null;

                ply.ReceiveHint("====== <color=red>ZETA WARHEAD</color> ======\n" +
                         "Zeta Warhead nie może być włączony podczas wybuchu alpha warhead.", 5);
                Timing.CallDelayed(2f, () => ZetaWarheadController.SpawnSecondActivateButton());
            }
            else if (item.name == "ZetaWarheadActivationButton1")
            {
                item.DestroySelf();
                ZetaWarheadController.ActivateBtn_1 = null;

                ply.ReceiveHint("====== <color=red>ZETA WARHEAD</color> ======\n" +
                         "Głowica zeta możliwa jest do uruchomienia dopiero po 15 minutach gry.", 5);
                Timing.CallDelayed(2f, () => ZetaWarheadController.SpawnFirstActivateButton());
            }
            else if (item.name == "ZetaWarheadActivationButton2")
            {
                item.DestroySelf();
                ZetaWarheadController.ActivateBtn_2 = null;

                ply.ReceiveHint("====== <color=red>ZETA WARHEAD</color> ======\n" +
                         "Głowica zeta możliwa jest do uruchomienia dopiero po 15 minutach gry.", 5);
                Timing.CallDelayed(2f, () => ZetaWarheadController.SpawnSecondActivateButton());
            }
        }

        [PluginEvent(PluginAPI.Enums.ServerEventType.PlayerChangeRole)]
        private void OnPlayerChangingRole(Player ply, PlayerRoleBase oldRole, RoleTypeId newRole, RoleChangeReason reason)
        {
            Timing.CallDelayed(1.5f, () =>
            {
                var e1 = ConfigHelper.GetRoomByRoomName("EZ_Endoof");
                var e2 = ConfigHelper.GetRoomByRoomName("EZ_Endoof (1)");
                if (ply.Room == e1 || ply.Room == e2)
                {
                    ply.Position = EntranceZone.Rooms.FirstOrDefault(x => x.Identifier.Name == RoomName.EzOfficeLarge).Position + Vector3.up * 2;
                }
            });
        }

        [PluginEvent(PluginAPI.Enums.ServerEventType.RoundRestart)]
        private void OnRoundRestart()
        {
            ZetaWarheadController.ActivatedButtons = 0;
            ZetaWarheadController.isDetonated = false;
            Timing.KillCoroutines(ZetaWarheadController.ZetaCoroutines.ToArray());
            ZetaWarheadController.ZetaCoroutines.Clear();
            ZetaWarheadController.ActivateBtn_1 = null;
            ZetaWarheadController.ActivateBtn_2 = null;
            ZetaWarheadController.O5_1 = null;
            ZetaWarheadController.O5_2 = null;
        }

        [PluginEvent(PluginAPI.Enums.ServerEventType.MapGenerated)]
        private void OnMapGenerated()
        {
            var ndoor = UnityEngine.Object.Instantiate(UnityEngine.Object.FindObjectsOfType<DoorSpawnpoint>().First(d => d.TargetPrefab.name.Contains("LCZ")).TargetPrefab, new UnityEngine.Vector3(37.848f, 1001.026f, -52.495f), Quaternion.Euler(0f, 90f, 0f));
            ndoor.RequiredPermissions.RequiredPermissions = KeycardPermissions.AlphaWarhead;
            ndoor.ServerChangeLock(DoorLockReason.SpecialDoorFeature, true);
            ndoor.name = "ZetaWarheadCancelRoomDoor";
            NetworkServer.Spawn(ndoor.gameObject);
            var roomname1 = "EZ_Endoof";
            var roomname2 = "EZ_Endoof (1)";

            var door1 = EntranceZone.Doors.FirstOrDefault(x => x.Room.Identifier.gameObject.name.ToLowerInvariant().Contains(roomname1.ToLowerInvariant()));
            if (door1 is not null)
                door1.Permissions = KeycardPermissions.AlphaWarhead;
            else
            {
                Log.Debug("t");
                EntranceZone.Doors.FirstOrDefault(x => x.Position.IsWithinDistance(ConfigHelper.GetRoomByRoomName(roomname1).transform.position, 15)).Permissions = KeycardPermissions.AlphaWarhead;
            }

            var door2 = EntranceZone.Doors.FirstOrDefault(x => x.Room.Identifier.gameObject.name.ToLowerInvariant().Contains(roomname2.ToLowerInvariant()));
            if (door2 is not null)
                door2.Permissions = KeycardPermissions.AlphaWarhead;
            else
            {
                Log.Debug("t");
                EntranceZone.Doors.FirstOrDefault(x => x.Position.IsWithinDistance(ConfigHelper.GetRoomByRoomName(roomname2).transform.position, 15)).Permissions = KeycardPermissions.AlphaWarhead;
            }
        }

        [PluginEvent(PluginAPI.Enums.ServerEventType.RoundStart)]
        private void OnRoundStart()
        {
            Log.Debug("Spawnowanie przycisków...");
            ZetaWarheadController.SpawnFirstActivateButton();
            ZetaWarheadController.SpawnSecondActivateButton();
        }

        [PluginEvent(PluginAPI.Enums.ServerEventType.RoundEnd)]
        private void onRoundEnd(LeadingTeam team)
        {
            if (ZetaWarheadController.AudioNpc is not null)
            {
                ZetaWarheadController.AudioNpc.Stop();
            }
        }

        private int stoppedcount;
        private void OnTrackEnd(AudioPlayerBase playerBase, string track, bool directPlay, ref int nextQueuePos)
        {
            if (playerBase == ZetaWarheadController.AudioNpc.AudioPlayerBase)
            {
                stoppedcount++;
                if (stoppedcount > 0)
                {
                    ZetaWarheadController.isDetonated = true;
                    ZetaWarheadController.AfterDetonationEffects();
                }
            }
        }
    }
}
