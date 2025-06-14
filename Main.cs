using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using RandomCardsGenerators.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RandomCardsGenerators {
    [BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("root.rarity.lib", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(modId, modName, "1.0.1")]
    [BepInProcess("Rounds.exe")]
    public class Main : BaseUnityPlugin {
        private const string modId = "com.aalund13.rounds.random_cards_generator";
        private const string modName = "Random Cards Generator";
        internal const string modInitials = "RCG";


        internal static Main instance;
        internal static ManualLogSource ModLogger;

        internal static AssetBundle assets;
        internal static GameObject blankCardPrefab;

        internal static Type RarityTextType;
        internal static Dictionary<string, float> ModRarities;

        private static Harmony harmony;

        void Awake() {
            instance = this;
            ModLogger = Logger;

            harmony = new Harmony(modId);
            harmony.PatchAll();

            assets = Jotunn.Utils.AssetUtils.LoadAssetBundleFromResources("randomcardsgenerator_assets", typeof(Main).Assembly);
            blankCardPrefab = assets.LoadAsset<GameObject>("__RCG__BlankCard");

            Debug.Log($"{modName} loaded!");
        }
        void Start() {
            ModdingUtils.Utils.Cards.instance.AddCardValidationFunction((player, card) => {
                if(ToggleCard.ToggleCards.Any(c => c.toggleCardInfo == card)) return false;
                if(!ToggleCard.IsCardEnabled(card)) return true;

                return true;
            });

            if(BepInEx.Bootstrap.Chainloader.PluginInfos.TryGetValue("pykess.rounds.plugins.deckcustomization", out var plugin)) {
                var assembly = plugin.Instance.GetType().Assembly;
                RarityTextType = assembly.GetType("DeckCustomization.RarityText");
                ModRarities = (Dictionary<string, float>)assembly.GetType("DeckCustomization.DeckCustomization").GetField("ModRarities", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).GetValue(null);
                DeckCustomizationPatch.Patch(harmony, assembly);
            }

            Debug.Log($"{modName} started!");
        }
    }
}