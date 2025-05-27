using System;
using System.Collections.Generic;
using UnboundLib;

namespace RandomCardsGenerators {
    public class ModRandomCardsGenerators {
        protected readonly Dictionary<string, RandomCardsGenerator> CardsGenerators = new Dictionary<string, RandomCardsGenerator>();

        public ModRandomCardsGenerators(List<RandomCardsGenerator> cardsGenerators) {
            foreach(var handler in cardsGenerators) {
                if(cardsGenerators == null || cardsGenerators.Count == 0)
                    throw new Exception("No handlers provided.");
                else if(CardsGenerators.ContainsKey(handler.CardGenName))
                    throw new Exception($"Handler for {handler.CardGenName} already exists.");

                CardsGenerators.Add(handler.CardGenName, handler);
            }
        }

        public void CreateRandomCard(string cardGenName, int seed, Player player = null) {
            var handler = GetHandler(cardGenName);
            handler.CreateRandomCard(seed, player);
        }
        public void CreateRandomCard(string cardGenName, Player player = null) {
            var handler = GetHandler(cardGenName);
            handler.CreateRandomCard(player);
        }

        public RandomCardsGenerator GetHandler(string statGenName) => CardsGenerators[statGenName];
        public IEnumerable<RandomCardsGenerator> GetAllHandlers() => CardsGenerators.Values;

        public void AddGenerator(string cardGenName, RandomCardsGenerator handler) {
            if(!CardsGenerators.ContainsKey(cardGenName)) {
                CardsGenerators.Add(cardGenName, handler);
            } else {
                throw new Exception($"Handler for {cardGenName} already exists.");
            }
        }
        public void RemoveGenerator(string cardGenName) {
            if(CardsGenerators.ContainsKey(cardGenName)) {
                CardsGenerators.Remove(cardGenName);
            }
        }
    }

    /// <summary>
    /// Generic version of ModRandomCardsGenerators for Enum types.
    /// <para>I highly recommend using this version instead of the non-generic one. Because it is more type-safe.</para>
    /// </summary>
    public class ModRandomCardsGenerators<T> : ModRandomCardsGenerators where T : Enum {
        protected readonly Dictionary<T, RandomCardsGenerator> TypedCardsGenerators = new Dictionary<T, RandomCardsGenerator>();

        public ModRandomCardsGenerators(Dictionary<T, RandomCardsGenerator> cardsGenerators) : base(new List<RandomCardsGenerator>(cardsGenerators.Values)) {
            foreach(var handler in cardsGenerators) {
                if(TypedCardsGenerators.ContainsKey(handler.Key))
                    throw new Exception($"Handler for {handler.Key} already exists.");

                TypedCardsGenerators.Add(handler.Key, handler.Value);
            }
        }

        public void CreateRandomCard(T stats, int seed, Player player = null) {
            var handler = GetHandler(stats);
            NetworkingManager.RaiseEvent(string.Format(RandomCardsGenerator.SYNC_EVENT_FORMAT, handler.CardGenName), seed, player?.playerID ?? -1);
        }
        public void CreateRandomCard(T stats, Player player = null) {
            var handler = GetHandler(stats);
            int seed = UnityEngine.Random.Range(0, int.MaxValue);
            NetworkingManager.RaiseEvent(string.Format(RandomCardsGenerator.SYNC_EVENT_FORMAT, handler.CardGenName), seed, player?.playerID ?? -1);
        }

        public RandomCardsGenerator GetHandler(T stat) => TypedCardsGenerators[stat];

        public void AddGenerator(T stat, RandomCardsGenerator handler) {
            if(!TypedCardsGenerators.ContainsKey(stat) && !CardsGenerators.ContainsKey(handler.CardGenName)) {
                TypedCardsGenerators.Add(stat, handler);
                CardsGenerators.Add(handler.CardGenName, handler);
            } else {
                throw new Exception($"Handler for {stat} already exists or the handler is already registered with another stat generator.");
            }
        }
        public void RemoveGenerator(T stat) {
            if(TypedCardsGenerators.ContainsKey(stat)) {
                TypedCardsGenerators.Remove(stat);
                CardsGenerators.Remove(stat.ToString());
            }
        }
    }
}
