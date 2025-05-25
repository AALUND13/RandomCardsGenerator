using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using UnityEngine;

namespace RandomCardsGenerators {
    [BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("root.rarity.lib", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(modId, modName, "1.0.0")]
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

        void Awake() {
            instance = this;
            ModLogger = Logger;
            new Harmony(modId).PatchAll();

            assets = Jotunn.Utils.AssetUtils.LoadAssetBundleFromResources("randomcardsgenerator_assets", typeof(Main).Assembly);
            blankCardPrefab = assets.LoadAsset<GameObject>("__RCG__BlankCard");

            Debug.Log($"{modName} loaded!");
        }
        void Start() {
            if(BepInEx.Bootstrap.Chainloader.PluginInfos.TryGetValue("pykess.rounds.plugins.deckcustomization", out var plugin)) {
                var assembly = plugin.Instance.GetType().Assembly;
                RarityTextType = assembly.GetType("DeckCustomization.RarityText");
            }

            Debug.Log($"{modName} started!");
        }
    }
}