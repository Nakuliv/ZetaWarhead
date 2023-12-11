using Axwabo.Helpers.Config;
using CustomPlayerEffects;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items;
using InventorySystem;
using LightContainmentZoneDecontamination;
using MapGeneration;
using MEC;
using Mirror;
using PlayerRoles;
using PluginAPI.Core;
using Respawning;
using SCPSLAudioApi.AudioCore;
using slocLoader.AutoObjectLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static LightContainmentZoneDecontamination.DecontaminationController;
using PluginAPI.Core.Items;
using InventorySystem.Items.Pickups;
using NightStars_Plugin.API.Dummy;

namespace ZetaWarhead
{
    public class ZetaWarheadController
    {
        public static NightStars_Plugin.API.Dummy.Npc AudioNpc = null;
        public static bool isDetonated = false;
        public static bool isStarted = false;
        public static bool is1ButtonSpawned;
        public static bool is2ButtonSpawned;
        public static int CanceledTimes;
        public static int ActivatedButtons;
        public static DecontaminationStatus previusDecontaminationStatus;
        public static ItemPickupBase O5_1 = null;
        public static ItemPickupBase O5_2 = null;
        public static ItemPickupBase ActivateBtn_1 = null;
        public static ItemPickupBase ActivateBtn_2 = null;
        public static List<CoroutineHandle> ZetaCoroutines = new List<CoroutineHandle>();
        public static void SpawnFirstActivateButton()
        {
            if (ActivateBtn_1 is not null) return;

            InventoryItemLoader.AvailableItems.TryGetValue(ItemType.KeycardJanitor, out ItemBase ButtonItem);

            var point = new PositionByRoomName("ZetaWActivate", new MapPointByName("EZ_Endoof", new SerializedRotation(0.005f - 0.0556f, 0.499f + 0.61f, -8.032f + 0.3800001f), new SerializedRotation(10)));
            var itempos = point.Location().WorldPose().position;

            var button1 = NightStars_Plugin.API.Extensions.CreatePickup(itempos, ButtonItem);
            button1.GetComponent<Rigidbody>().isKinematic = true;
            button1.name = "ZetaWarheadActivationButton1";
            ActivateBtn_1 = button1;
            NetworkServer.Spawn(button1.gameObject);
        }

        public static void SpawnSecondActivateButton()
        {
            if (ActivateBtn_2 is not null) return;

            InventoryItemLoader.AvailableItems.TryGetValue(ItemType.KeycardJanitor, out ItemBase ButtonItem);

            var point = new PositionByRoomName("ZetaWActivate", new MapPointByName("EZ_Endoof (1)", new SerializedRotation(0.005f - 0.0556f, 0.499f + 0.61f, -8.032f + 0.3800001f), new SerializedRotation(10)));
            var itempos = point.Location().WorldPose().position;

            var button1 = NightStars_Plugin.API.Extensions.CreatePickup(itempos, ButtonItem);
            button1.GetComponent<Rigidbody>().isKinematic = true;
            button1.name = "ZetaWarheadActivationButton2";
            ActivateBtn_2 = button1;
            NetworkServer.Spawn(button1.gameObject);
        }

        public static void Activate()
        {
            AlphaWarheadController.Singleton.IsLocked = true;
            isStarted = true;
            Facility.TurnOffAllLights(1);

            ZetaCoroutines.Add(
            Timing.CallDelayed(5.5f, () =>
            {
                Facility.TurnOffAllLights(1);
                foreach (RoomLightController instance in RoomLightController.Instances)
                {
                    //Color warheadLightColor = new Color(97 / 255f, 222 / 255f, 42 / 255f);
                    Color warheadLightColor = new Color(255 / 255f, 139 / 255f, 0 / 255f);
                    instance.NetworkOverrideColor = warheadLightColor;
                    //instance.WarheadLightOverride = true;
                }
            }));

            //Cassie.Message("pitch_0.15 .G4 . .G4 pitch_0.89 ATTENTION PITCH_0.93 ZETA WARHEAD DETONATION SEQUENCE HAS BEEN INITIATED . THE SURFACE ZONE WILL BE DETONATED IN TMINUS 1 MINUTE . PITCH_0.95 ALL PERSONNEL MUST EVACUATE INSIDE THE FACILITY IMMEDIATELY", isNoisy: false);
            Cassie.Message("pitch_0.15 .g4 . .g4 pitch_0.85 . DANGER . pitch_0.92 THE jam_021_2 .g4 ZETA WARHEAD jam_021_2 DETONATION .g4 SEQUENCE HAS BEEN jam_021_2.g3 INITIATED . SURFACE ZONE . ENTRANCE ZONE .g3 .g5 AND jam_021_2 HEAVY .g5 CONTAIMENT ZONE WILL BE jam_021_2 DETONATED . IN TMINUS 2 MINUTES . LIGHT jam_021_2 CONTAINMENT ZONE OVERALL DECONTAMINATION jam_021_2 SEQUENCE .g6 .g1 IS NOW jam_021_2 DISENGAGED . EVACUATE . NOW", isNoisy: false);
            Cassie.Message("UWAGA\u200BDetonacja\u200BGłowicy\u200BZeta\u200Bzostała\u200Baktywowana,\u200Bpowierzchnia,\u200BEntrance\u200BZone\u200Bi\u200BHeavy\u200BContainment\u200BZone\u200Bzostaną\u200Bzdetonowane\u200Bw\u200Bciągu\u200B2\u200Bminut\u200B,\u200BCały\u200Bpersonel\u200Bproszony\u200Bjest\u200Bo\u200Bnatychmiastową\u200Bewakuacje\u200Bdo\u200BLight\u200BContainment\u200BZone<size=0> . </size>", false, false, true);
            ReverseDecon();

            ZetaCoroutines.Add(Timing.CallDelayed(18f, () =>
            {
                if (AudioNpc is null)
                {
                    var npc = NightStars_Plugin.API.Dummy.Npc.Create(Vector3.one, "C.A.S.S.I.E", role: RoleTypeId.None);
                    AudioNpc = npc;
                    npc.Play("/home/nsadmin/.config/SCP Secret Laboratory/PluginAPI/plugins/7779/zetaw.ogg", VoiceChat.VoiceChatChannel.RoundSummary, 10);
                }
                else
                {
                    AudioNpc.Play("/home/nsadmin/.config/SCP Secret Laboratory/PluginAPI/plugins/7779/zetaw.ogg", VoiceChat.VoiceChatChannel.RoundSummary, 10);
                }

                OpenAndLockDoors();
            }));
            if (CanceledTimes == 1 || CanceledTimes < 1)
            {
                InventoryItemLoader.AvailableItems.TryGetValue(ItemType.KeycardJanitor, out ItemBase ButtonItem);

                var point0 = new PositionByRoomName("ZetaWD", new MapPointByName("Outside", new SerializedRotation(41.080f, 1.476f + 0.9f, -51.23f), new SerializedRotation(10)));
                var itempos0 = point0.Location().WorldPose().position;
                var button = NightStars_Plugin.API.Extensions.CreatePickup(itempos0, ButtonItem);
                button.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                button.GetComponent<Rigidbody>().isKinematic = true;
                button.name = "ZetaWarheadCancelButton";
                NetworkServer.Spawn(button.gameObject);
            }
        }
        public static void ReverseDecon()
        {
            previusDecontaminationStatus = DecontaminationController.Singleton.NetworkDecontaminationOverride;
            DecontaminationController.Singleton.NetworkDecontaminationOverride = DecontaminationController.DecontaminationStatus.Disabled;
            foreach (Player ply in Player.GetPlayers().Where(x => x.Zone == MapGeneration.FacilityZone.LightContainment))
            {
                ply.EffectsManager.DisableEffect<Decontaminating>();
            }
            foreach (var edoors in ElevatorDoor.AllElevatorDoors)
            {
                foreach (var edoor in edoors.Value)
                {
                    edoor.ServerChangeLock(DoorLockReason.DecontLockdown, false);
                }
            }

            foreach (var door in DoorVariant.AllDoors.Where(x => x.GetComponentInParent<AirlockController>() != null))
            {
                door.ServerChangeLock(DoorLockReason.DecontLockdown, false);
            }
        }

        public static void AfterDetonationEffects()
        {

            foreach (var edoors in ElevatorDoor.AllElevatorDoors)
            {
                foreach (var edoor in edoors.Value.Where(x => !x.IsInZone(FacilityZone.LightContainment)))
                {
                    edoor.ServerChangeLock(DoorLockReason.Warhead, true);
                }
            }

            AlphaWarheadController.Singleton.RpcShake(false);
            //NetworkServer.UnSpawn(DummyAudio.gameObject);

            foreach (Player ply in Player.GetPlayers().Where(x => x.Zone != FacilityZone.LightContainment))
            {
                ply.Kill("zginął podczas wybuchu <color=red>ZETA WARHEAD</color>");
            }

            ZetaCoroutines.Add(Timing.CallDelayed(5f, () =>
            {
                Facility.TurnOffAllLights(1);
                foreach (RoomLightController instance in RoomLightController.Instances)
                {
                    instance.NetworkOverrideColor = Color.white;
                }
            }));
        }

        public static void Cancel()
        {
            if (CanceledTimes < 1)
            {
                CanceledTimes++;
                AlphaWarheadController.Singleton.IsLocked = false;
                isStarted = false;
                ActivatedButtons = 0;

                if (O5_1 is not null) O5_1.DestroySelf();
                if (O5_2 is not null) O5_2.DestroySelf();

                Cassie.Message("pitch_0.93 jam_044_2 DETONATION .G2 OF THE ZETA jam_044_2 WARHEAD HAS BEEN jam_044_2 DEACTIVATED .G3 .G2");
                Cassie.Message("UWAGA\u200BDetonacja\u200BGłowicy\u200BZeta\u200Bzostała\u200Banulowana.<size=0> . </size>", false, false, true);
                DecontaminationController.Singleton.NetworkDecontaminationOverride = previusDecontaminationStatus;
                foreach (var door in DoorVariant.AllDoors.Where(x => !x.IsInZone(FacilityZone.LightContainment) && x is not ElevatorDoor))
                {
                    if (door.name != "ZetaWarheadCancelRoomDoor")
                    {
                        door.ServerChangeLock(DoorLockReason.Warhead, false);
                    }
                    else
                    {
                        ZetaCoroutines.Add(Timing.CallDelayed(10f, () =>
                        {
                            door.NetworkTargetState = false;
                            door.ServerChangeLock(DoorLockReason.SpecialDoorFeature, true);
                        }));
                    }
                }
                //NetworkServer.UnSpawn(DummyAudio.gameObject);
                AudioNpc.Stop();

                Facility.TurnOffAllLights(1);
                foreach (RoomLightController instance in RoomLightController.Instances)
                {
                    instance.NetworkOverrideColor = Color.white;
                }

                ZetaCoroutines.Add(Timing.CallDelayed(2f, () =>
                {
                    InventoryItemLoader.AvailableItems.TryGetValue(ItemType.KeycardJanitor, out ItemBase ButtonItem);

                    var point = new PositionByRoomName("ZetaWActivate", new MapPointByName("EZ_Endoof", new SerializedRotation(0.005f - 0.0556f, 0.499f + 0.61f, -8.032f + 0.3800001f), new SerializedRotation(10)));
                    var itempos = point.Location().WorldPose().position;

                    var button1 = NightStars_Plugin.API.Extensions.CreatePickup(itempos, ButtonItem);
                    button1.GetComponent<Rigidbody>().isKinematic = true;
                    button1.name = "ZetaWarheadActivationButton1";
                    NetworkServer.Spawn(button1.gameObject);

                    var point2 = new PositionByRoomName("ZetaWActivate", new MapPointByName("EZ_Endoof (1)", new SerializedRotation(0.005f - 0.0556f, 0.499f + 0.61f, -8.032f + 0.3800001f), new SerializedRotation(10)));
                    var itempos2 = point2.Location().WorldPose().position;

                    var button2 = NightStars_Plugin.API.Extensions.CreatePickup(itempos2, ButtonItem);
                    button2.GetComponent<Rigidbody>().isKinematic = true;
                    button2.name = "ZetaWarheadActivationButton2";
                    NetworkServer.Spawn(button2.gameObject);
                }));
            }
        }

        public static void OpenAndLockDoors()
        {
            foreach (var door in DoorVariant.AllDoors.Where(x => !x.IsInZone(FacilityZone.LightContainment) && x is not ElevatorDoor))
            {
                if (door.name != "ZetaWarheadCancelRoomDoor")
                {
                    door.ServerChangeLock(DoorLockReason.Warhead, true);
                    door.NetworkTargetState = true;
                }
                else
                {
                    door.NetworkTargetState = false;
                    ZetaCoroutines.Add(Timing.CallDelayed(25f, () =>
                    {
                        door.ServerChangeLock(DoorLockReason.SpecialDoorFeature, false);
                    }));
                }
            }
        }
    }
}
