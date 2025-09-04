using BepInEx;
using BepInEx.Logging;
using BepInEx.NET.Common;
using BepInExResoniteShim;
using FrooxEngine;
using HarmonyLib;
using System.Reflection.Emit;

namespace ShowUserspaceSpawnButton
{
    [ResonitePlugin(PluginMetadata.GUID, PluginMetadata.NAME, PluginMetadata.VERSION, PluginMetadata.AUTHORS, PluginMetadata.REPOSITORY_URL)]
    [BepInDependency(BepInExResoniteShim.PluginMetadata.GUID)]
    public class ShowUserspaceSpawnButton : BasePlugin
    {
        public override void Load() => HarmonyInstance.PatchAll();

        [HarmonyPatch(typeof(InventoryBrowser), "OnItemSelected")]
        class ShowUserspaceSpawnButtonPatch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = instructions.ToList();
                for (var i = 0; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldloc_1 && codes[i + 1].opcode == OpCodes.Ldc_I4_5 && codes[i + 5].opcode == OpCodes.Brfalse) //comparing specialItemType to SpecialItemType.Facet
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
            [HarmonyPostfix]
            [HarmonyPatch("OnAttach")]
            static void OnAttachPostFixSync(Sync<InventoryBrowser.SpecialItemType> ____lastSpecialItemType)
            {
                ____lastSpecialItemType.Value = (InventoryBrowser.SpecialItemType)(-1);
            }
        }
    }
}