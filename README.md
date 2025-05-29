# RandomCardsGenerator
A library for generating random cards with random stats.   
For example, you can take a look at the [Corrupted Cards Manager](https://github.com/AALUND13/CorruptedCardsManager) which uses this library.

## Table of Contents
- [Getting Started](#getting-started)
- [Managing Multiple Generators](#managing-multiple-generators)
- [Custom Stat Generators](#custom-stat-generators)
- [Event Handling](#event-handling)
- [Built-in Stat Generators](#built-in-stat-generators)
- [Making Cards Appear In-Game](#making-cards-appear-in-game)

# Getting Started
## Understanding the `RandomCardsGenerator` class
The `RandomCardsGenerator` class is the part of the library that generates random cards. Let explain how it works.   

First we need to create the list of `RandomStatGenerator` to pass to the `RandomCardsGenerator` constructor.
For full list of the available stat generators, see the [Built-in Stat Generators](#built-in-stat-generators) section.
```csharp
List<RandomStatGenerator> statGenerators = new List<RandomStatGenerator>
{
    // If this stat is picked, it will generate a random damage value between -30% and +70% of the damage
    new DamageStatGenerator(-0.3f, 0.7f),
    // If this stat is picked, it will generate a random fire rate value between -30% and +30% of the fire rate
    new ReloadTimeStatGenerator(-0.3f, 0.3f),
    // If this stat is picked, it will generate a random health value between -30% and +70% of the health
    new HealthStatGenerator(-0.3f, 0.7f),
    // If this stat is picked, it will generate a random movement speed value between -20% and +40% of the movement speed
    new MovementSpeedStatGenerator(-0.2f, 0.4f)
};
```

Now we need to create the `RandomCardOption`. 
This is the class that defines the card options.
```csharp
RandomCardOption cardOption = new RandomCardOption(
    "Example Random Card",       // Name for the cards
    CardInfo.Rarity.Uncommon,    // Card rarity
    "RCG",                       // Mod initials shown on card
    "A randomly generated card", // Card description
    "Ex",                        // Two letter code for card
    2,                           // Minimum number of stats to apply
    4                            // Maximum number of stats to apply
);
```

Finally we can create the `RandomCardsGenerator` instance.
```csharp
RandomCardsGenerator randomCardsGenerator = new RandomCardsGenerator(
    "ExampleRandomCardsGenerator", // The identifier for the generator, It must be unique
    cardOption,                    // Card option
    statGenerators                 // List of stat generators
);
```

Now to generate a random card we can use the `CreateRandomCard` method.
```csharp
randomCardsGenerator.CreateRandomCard(player);
// You also can pass a `seed` parameter to the method to generate the same card every time
randomCardsGenerator.CreateRandomCard(seed, player);
```

## Managing Multiple Generators
If you have multiple `RandomCardsGenerator` instances, you can manage them using the `ModRandomCardsGenerators` class. There two types of `ModRandomCardsGenerators`:
- `ModRandomCardsGenerators`: Non generic class that can manage multiple `RandomCardsGenerator` instances.
- `ModRandomCardsGenerators<T>`: Generic class that can manage multiple `RandomCardsGenerator` instances using enum values as keys, where `T` is the type of the enum. Helpful for type-safe access to generators.  

Non generic example:
```csharp
// Create a `RandomCardsGenerator` instance
var modRandomCardsGenerators = new ModRandomCardsGenerators(
  new List<RandomCardsGenerator> {
      randomCardsGenerator1,
      randomCardsGenerator2,
      randomCardsGenerator3
  }
);

// Now you can use the `ModRandomCardsGenerators` instance to generate random cards
modRandomCardsGenerators.CreateRandomCard("ExampleRandomCardsGenerator1", player);
```

Generic example:
```csharp
// Create a enum for the generators
public enum ExampleRandomCardsGenerators {
    Generator1,
    Generator2,
    Generator3
}

// Create a `modRandomCardsGenerators` instance
var modRandomCardsGenerators = new ModRandomCardsGenerators<ExampleRandomCardsGenerators>(
    new Dictionary<ExampleRandomCardsGenerators, RandomCardsGenerator> {
        { ExampleRandomCardsGenerators.Generator1, randomCardsGenerator1 },
        { ExampleRandomCardsGenerators.Generator2, randomCardsGenerator2 },
        { ExampleRandomCardsGenerators.Generator3, randomCardsGenerator3 }
    }
);

// Now you can use the `ModRandomCardsGenerators` instance to generate random cards
modRandomCardsGenerators.CreateRandomCard(ExampleRandomCardsGenerators.Generator1, player);
```

## Custom Stat Generators
You can create your own custom stat generators by inheriting from the `RandomStatGenerator` class.  
For a example we going show the `DamageStatGenerator` but simplified and with added comments to explain how it works.
```csharp
// First we need to inherit from the `RandomStatGenerator` class
public class ExampleDamageStatGenerator : RandomStatGenerator {
    // The name of the stat, this will be shown on the card
    public override string StatName => "Damage";
    public float ThresholdToZero;

    // Constructor to initialize the stat generator with min and max values
    public DamageStatGenerator(float minValue, float maxValue, float thresholdToZero = 0.05f) : base(minValue, maxValue) {
        ThresholdToZero = thresholdToZero;
    }

    // This method is optional, it defines if the stat should be applied based on the value
    // We using it to check if the absolute value of the stat is greater than or equal to the threshold
    public override bool ShouldApply(float value) => Mathf.Abs(value) >= ThresholdToZero;

    // This method is called to apply the stat to the player.
    public override void Apply(float value, CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block) =>
        gun.damage += value;

    // This method is defines if the stat value is positive or negative
    public override bool IsPositive(float value) => value > 0;
}
```

Now you can use the `ExampleDamageStatGenerator` just like the other stat generators.
```csharp
List<RandomStatGenerator> statGenerators = new List<RandomStatGenerator> {
    new ExampleDamageStatGenerator(-0.3f, 0.7f) // Value between -30% and +70% of the damage
};
```

## Event Handling
You can subscribe to events to be notified when a card is generated. This is useful for modifying the card after it's generated or keeping track of generated cards.

```csharp
// Subscribe to the OnCardGenerated event
randomCardsGenerator.OnCardGenerated += (GeneratedCardInfo generatedCardInfo) => {
    // This event is triggered after a card is generated
    // You can access the generated card info and perform additional actions
    UnityEngine.Debug.Log($"Generated card: {generatedCardInfo.CardInfo.cardName}");
    
    // You can also access the random values used for each stat
    foreach (var statInfo in generatedCardInfo.RandomStatInfos) {
        UnityEngine.Debug.Log($"Stat: {statInfo.StatGenerator.StatName}, Value: {statInfo.Value}");
    }
};
```

## Built-in Stat Generators
The library comes with several built-in stat generators that you can use out of the box. 
Here is a list of the available stat generators:  
- `ReloadTimeSecondStatGenerator`
- `BlockCooldownSecondStatGenerator`
- `NumberOfProjectilesStatGenerator`
- `BouncesStatGenerator`
- `AmmoStatGenerator`
- `AdditionalBlocksStatGenerator`
- `ExtraLiveStatGenerator`
- `JumpStatGenerator`
- `HealthStatGenerator`
- `SizeStatGenerator`
- `DamageStatGenerator`
- `ReloadTimeStatGenerator`
- `AttackSpeedStatGenerator`
- `MovementSpeedStatGenerator`
- `BlockCooldownStatGenerator`
- `BulletSpeedStatGenerator`
- `RegenStatGenerator`
- `SpreadStatGenerator`
- `DragStatGenerator`
- `LifeSteelStatGenerator`

## Making Cards Appear In-Game
The library provides a convenient way to make your random cards appear as physical cards in the game using the `DrawableRandomStatsCard` class. This is essential for creating pickable cards that appear in players' hands.

### Using DrawableRandomStatsCard

```csharp
// Create a drawable random stats card using your generator
var drawableCard = new DrawableRandomStatsCard(randomCardsGenerator);

// Replace an existing card with a random one
CardInfo existingCard = someExistingCard;
GameObject newCardObject = drawableCard.ReplaceCard(existingCard);
```

### Implementation Notes

To integrate random card generation into your mod's card selection system, you'll likely need additional code. For a complete example, see the [Corrupted Cards Manager](https://github.com/AALUND13/CorruptedCardsManager) which implements:

1. Card spawning in player hands
2. Custom card appearance
3. Integration with the game's card selection system

Specifically, look at these files in that project:
- [SpawnUniqueCardPatch.cs](https://github.com/AALUND13/CorruptedCardsManager/blob/main/Scripts/Patches/SpawnUniqueCard.cs)
- [CorruptedCardsGenerators.cs](https://github.com/AALUND13/CorruptedCardsManager/blob/main/Scripts/CorruptedCardsGenerators.cs)