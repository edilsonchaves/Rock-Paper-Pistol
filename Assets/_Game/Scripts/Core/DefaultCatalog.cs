using System.Collections.Generic;

namespace RockPaperPistol.Core
{
    public readonly struct NamedDeck
    {
        public NamedDeck(string name, IReadOnlyList<Card> cards)
        {
            Name = name;
            Cards = cards;
        }

        public string Name { get; }
        public IReadOnlyList<Card> Cards { get; }
    }

    public readonly struct NamedEnemy
    {
        public NamedEnemy(string name, IReadOnlyList<Card> sequence)
            : this(name, sequence, EnemyBehavior.Defensive)
        {
        }

        public NamedEnemy(string name, IReadOnlyList<Card> sequence, EnemyBehavior behavior)
        {
            Name = name;
            Sequence = sequence;
            Behavior = behavior;
        }

        public string Name { get; }
        public IReadOnlyList<Card> Sequence { get; }
        public EnemyBehavior Behavior { get; }
    }

    public static class DefaultCatalog
    {
        public const string PlayerName = "Pistoleiro";
        public const string BaseDeckName = "Baralho Base";
        public const int BaseDeckSize = 9;

        public static readonly IReadOnlyList<Card> BaseDeck = new[]
        {
            new Card(Suit.Rock, 1),
            new Card(Suit.Rock, 2),
            new Card(Suit.Rock, 3),
            new Card(Suit.Paper, 1),
            new Card(Suit.Paper, 2),
            new Card(Suit.Paper, 3),
            new Card(Suit.Scissors, 1),
            new Card(Suit.Scissors, 2),
            new Card(Suit.Scissors, 3)
        };

        public static readonly IReadOnlyList<NamedDeck> Decks = new[]
        {
            new NamedDeck(BaseDeckName, BaseDeck)
        };

        public static readonly IReadOnlyList<NamedEnemy> Enemies = new[]
        {
            new NamedEnemy("Estátua de Pedra", BaseDeck, EnemyBehavior.Defensive),
            new NamedEnemy("Múmia", BaseDeck, EnemyBehavior.Defensive),
            new NamedEnemy("Pirata", BaseDeck, EnemyBehavior.Aggressive)
        };

        public static IReadOnlyList<IReadOnlyList<Card>> EnemySequences
        {
            get
            {
                return new IReadOnlyList<Card>[]
                {
                    BaseDeck,
                    BaseDeck,
                    BaseDeck
                };
            }
        }
    }
}
