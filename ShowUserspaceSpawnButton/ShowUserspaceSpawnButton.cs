using HarmonyLib;
using NeosModLoader;
using FrooxEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;

namespace ShowUserspaceSpawnButton
{
    public class ShowUserspaceSpawnButton : NeosMod
    {
        public override string Name => "ShowUserspaceSpawnButton";
        public override string Author => "eia485";
        public override string Version => "1.0.0";
        public override string Link => "https://github.com/EIA485/NeosShowUserspaceSpawnButton/";
        public override void OnEngineInit() =>new Harmony("net.eia485.ShowUserspaceSpawnButton").PatchAll();

        [HarmonyPatch(typeof(InventoryBrowser), "OnItemSelected")]
        class ShowUserspaceSpawnButtonPatch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = instructions.ToList();
                for (var i = 0; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldloc_1 && codes[i + 1].opcode == OpCodes.Ldc_I4_5 && codes[i + 5].opcode == OpCodes.Brfalse_S) //comparing specialItemType to SpecialItemType.Facet
                    {
                        //checking if item is null
                        codes[i] = new CodeInstruction(OpCodes.Ldloc_0);//load item ui
                        codes[i + 1] = codes[i + 5]; //jump if false(null ptr)
                        //removing unnecessary instructions
                        codes.RemoveRange(i + 2, 4);
                        break; //only patch first occurrence
                    }
                }
                return codes.AsEnumerable();
            }
        }
    }
}