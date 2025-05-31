using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RandomCardsGenerators.Utils {
    public class FindRandomCardsGeneratorResult {
        public readonly int Seed;
        public readonly RandomCardsGenerator RandomCardsGenerator;
        public FindRandomCardsGeneratorResult(int seed, RandomCardsGenerator randomCardsGenerator) {
            Seed = seed;
            RandomCardsGenerator = randomCardsGenerator;
        }
    }

    public static class RandomCardsUtils {
        public static Regex randomCardRegex = new Regex(@"^___RANDOM___(.*?)_(?:\((\d+)\))?$", RegexOptions.Compiled);

        public static FindRandomCardsGeneratorResult FindRandomCardsGeneratorByName(string cardName) {
            var match = randomCardRegex.Match(cardName);
            if(match.Success) {
                string generatorName = match.Groups[1].Value;
                int seed;
                if(match.Groups.Count > 2 && int.TryParse(match.Groups[2].Value, out int parsedSeed)) {
                    seed = parsedSeed;
                } else {
                    return null;
                }

                var randomCardsGenerator = RandomCardsGenerator.RandomStatCardGenerators.FirstOrDefault(r => r.Key == generatorName).Value;
                if(randomCardsGenerator != null) {
                    return new FindRandomCardsGeneratorResult(seed, randomCardsGenerator);
                }
            }
            return null;
        }
    }
}
