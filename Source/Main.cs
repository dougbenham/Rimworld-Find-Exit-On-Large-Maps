using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace FixedExitOnLargeMaps
{
    [StaticConstructorOnStartup]
    static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            var harmony = new Harmony("doug.FixedExitOnLargeMaps");
            harmony.PatchAll();
        }
    }
    
    [HarmonyPatch(typeof(RCellFinder), nameof(RCellFinder.TryFindBestExitSpot))]
    public static class RCellFinder_TryFindBestExitSpot_Patch
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Patch(IEnumerable<CodeInstruction> instructions)
        {
            var success = false;
            foreach (var i in instructions)
            {
                if (i.opcode == OpCodes.Ldc_I4_S && i.operand is sbyte s && s == 30)
                {
                    i.operand = 90;
                    success = true;
                    Log.Message("[FixedExitOnLargeMaps] Patched exit finder logic.");
                }
                yield return i;
            }
            if (!success)
                Log.Error("[FixedExitOnLargeMaps] Failed to patch exit finder logic!");
        }
    }
}
