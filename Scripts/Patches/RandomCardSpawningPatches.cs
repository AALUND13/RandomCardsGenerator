using HarmonyLib;
using ModdingUtils.Patches;
using Photon.Pun;
using RandomCardsGenerators.Cards;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RandomCardsGenerators.Patches {
    [HarmonyPatch(typeof(CardChoice), "Spawn")]
    public class SpawnPatch {
        [HarmonyPriority(Priority.Last)]
        private static bool Prefix(GameObject objToSpawn, Vector3 pos, Quaternion rot, ref GameObject __result) {
            if(objToSpawn != null && objToSpawn.GetComponent<RandomCard>() != null) {
                __result = PhotonNetwork.Instantiate(
                    objToSpawn.name,
                    pos,
                    rot,
                    0,
                    new object[] { DrawableRandomStatsCard.random.Next(int.MaxValue) }
                );
                return false; // Skip original method
            }
            return true; // Continue with original method
        }
    }

    [HarmonyPatch(typeof(CardChoice), "SpawnUniqueCard")]
    public class SpawnUniqueCardPatch {
        // Mainly used to detect if the card spawning is in the pick phase
        public static bool PickPhaseCardSpawning = false;

        [HarmonyPriority(Priority.First)] private static void Prefix() => PickPhaseCardSpawning = true;
        [HarmonyPriority(Priority.First)] private static void Postfix() => PickPhaseCardSpawning = false;

    }

    [HarmonyPatch(typeof(CardChoicePatchGetRanomCard), "OrignialGetRanomCard", new Type[] { typeof(CardInfo[]) })]
    public class CardChoicePatchGetRanomCardPatch {
        private static void Prefix(ref CardInfo[] cards) {
            if (!SpawnUniqueCardPatch.PickPhaseCardSpawning) return;

            List<CardInfo> list = new List<CardInfo>(cards);
            foreach(var drawableNormalCard in NormalDrawableRandomStatsCard.NormalDrawableCards) {
                list.Add(drawableNormalCard.CardInfo);
            }
            cards = list.ToArray();
        }
    }
}
