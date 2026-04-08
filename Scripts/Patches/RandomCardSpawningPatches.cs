using HarmonyLib;
using ModdingUtils.Patches;
using Photon.Pun;
using PickPhaseImprovements;
using RandomCardsGenerators.Cards;
using RandomCardsGenerators.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnboundLib.Utils;
using UnityEngine;

namespace RandomCardsGenerators.Patches {
    [HarmonyPatch]
    public class RandomCardSpawningPatches {
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
            Player player = (((PickerType)CardChoice.instance.GetFieldValue("pickerType") != 0)
                ? PlayerManager.instance.players[CardChoice.instance.pickrID]
                : PlayerManager.instance.GetPlayersInTeam(CardChoice.instance.pickrID)[0]);

            List<CardInfo> list = new List<CardInfo>(cards);
            foreach(var drawableNormalCard in NormalDrawableRandomCard.NormalDrawableCards) {
                if(list.Contains(drawableNormalCard.ToggleCard.toggleCardInfo)) {
                    list.Remove(drawableNormalCard.ToggleCard.toggleCardInfo);
                    list.Add(drawableNormalCard.CardInfo);
                }
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
