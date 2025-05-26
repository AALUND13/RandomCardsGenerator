using HarmonyLib;
using System;
using System.Reflection;

namespace RandomCardsGenerators {
    internal class DeckCustomizationPatch {
        public static void Patch(Harmony harmony, Assembly assembly) {
            harmony.Patch(assembly.GetType("DeckCustomization.RarityUtils").GetMethod("GetRelativeRarity", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(string) }, null),
                prefix: new HarmonyMethod(typeof(DeckCustomizationPatch), nameof(GetRelativeRarityPrefix)));
        }

        public static bool GetRelativeRarityPrefix(string modName, ref float __result) {
            if(!Main.ModRarities.ContainsKey(modName)) {
                __result = 0.5f;
                return false;
            }
            return true;
        }
    }
}