using HarmonyLib;
using Respawning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZetaWarhead.Patches
{
    [HarmonyPatch(typeof(RespawnManager), nameof(RespawnManager.Spawn))]
    internal static class RespawnPatch
    {
        [HarmonyPrefix]
        public static bool Spawn()
        {
            if (ZetaWarheadController.isDetonated) return false;
            else return true;
        }
    }
}
