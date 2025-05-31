using HarmonyLib;
using ModdingUtils.Patches;
using Photon.Pun;
using RandomCardsGenerators.Cards;
using RandomCardsGenerators.Utils;
using System;
using System.Collections.Generic;
using UnboundLib.Utils;
using UnityEngine;

namespace RandomCardsGenerators.Patches {
    [HarmonyPatch]
    public class RandomCardSpawningPatches {
        [HarmonyPatch(typeof(CardChoice), "SpawnUniqueCard")]
        class SpawnUniqueCardPatch {
            // Mainly used to detect if the card spawning is in the pick phase
            public static bool PickPhaseCardSpawning = false;

            [HarmonyPriority(Priority.First)] private static void Prefix() => PickPhaseCardSpawning = true;
            [HarmonyPriority(Priority.First)] private static void Postfix() => PickPhaseCardSpawning = false;
        }

        [HarmonyPatch(typeof(CardChoice), "Spawn")]
        [HarmonyPriority(Priority.Last)]
        [HarmonyPrefix]
        private static bool Spawn(GameObject objToSpawn, Vector3 pos, Quaternion rot, ref GameObject __result) {
            if(objToSpawn != null && objToSpawn.GetComponent<RandomCard>() != null) {
                __result = PhotonNetwork.Instantiate(
                    objToSpawn.name,
                    pos,
                    rot,
                    0,
                    new object[] { DrawableRandomCard.random.Next(int.MaxValue) }
                );
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(ModdingUtils.Utils.Cards), "AddCardToPlayer", new Type[] { typeof(Player), typeof(CardInfo), typeof(bool), typeof(string), typeof(float), typeof(float), typeof(bool) })]
        [HarmonyPrefix]
        private static bool AddRandomCardToPlayer(Player player, CardInfo card) {
            if(card.GetComponent<ToggleCustomCard>() != null && (PhotonNetwork.OfflineMode || PhotonNetwork.IsMasterClient)) {
                foreach(var drawableCard in NormalDrawableRandomCard.NormalDrawableCards) {
                    if(drawableCard.ToggleCard != null && drawableCard.ToggleCard.toggleCardInfo == card) {
                        drawableCard.StatCardGenerator.CreateRandomCard(player);
                        return false;
                    }
                }
            }

            return true;
        }

        [HarmonyPatch(typeof(ModdingUtils.Utils.Cards), "RPCA_AssignCard", new Type[] { typeof(string), typeof(int), typeof(bool), typeof(string), typeof(float), typeof(float), typeof(bool) })]
        [HarmonyPrefix]
        public static void AssignRandomCardRPC(string cardObjectName, int playerID, bool reassign, string twoLetterCode, float forceDisplay, float forceDisplayDelay, bool addToCardBar) {
            FindRandomCardsGeneratorResult findResult = RandomCardsUtils.FindRandomCardsGeneratorByName(cardObjectName);
            if(findResult != null) {
                Player playerToUpgrade;
                playerToUpgrade = PlayerManager.instance.players.Find(p => p.playerID == playerID);
                findResult.RandomCardsGenerator.GenerateRandomCard(findResult.Seed);
            }
        }

        [HarmonyPatch(typeof(CardChoicePatchGetRanomCard), nameof(CardChoicePatchGetRanomCard.OrignialGetRanomCard), new Type[] { typeof(CardInfo[]) })]
        [HarmonyPrefix]
        private static void NormalDrawableCardsSpawn(ref CardInfo[] cards) {
            if(!SpawnUniqueCardPatch.PickPhaseCardSpawning) return;

            List<CardInfo> list = new List<CardInfo>(cards);
            foreach(var drawableNormalCard in NormalDrawableRandomCard.NormalDrawableCards) {
                list.Add(drawableNormalCard.CardInfo);
            }
            cards = list.ToArray();
        }

        [HarmonyPatch(typeof(CardManager), nameof(CardManager.GetCardInfoWithName))]
        [HarmonyPostfix]
        public static void GetRandomCardInfoWithName(string cardName, ref CardInfo __result) {
            FindRandomCardsGeneratorResult findResult = RandomCardsUtils.FindRandomCardsGeneratorByName(cardName);
            if(findResult != null) {
                __result = findResult.RandomCardsGenerator.GenerateRandomCard(findResult.Seed).GetComponent<CardInfo>();
            }
        }
    }
}
