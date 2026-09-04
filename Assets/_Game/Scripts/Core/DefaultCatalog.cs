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

        public static readonly IReadOnlyList<Card> Equilibrado = new[]
        {
            new Card(Suit.Rock, 1),
            new Card(Suit.Rock, 3),
            new Card(Suit.Rock, 4),
            new Card(Suit.Paper, 2),
            new Card(Suit.Paper, 3),
            new Card(Suit.Paper, 5),
            new Card(Suit.Scissors, 1),
            new Card(Suit.Scissors, 3),
            new Card(Suit.Scissors, 4)
        };

        public static readonly IReadOnlyList<Card> Agressivo = new[]
        {
            new Card(Suit.Rock, 4),
            new Card(Suit.Rock, 5),
            new Card(Suit.Paper, 3),
            new Card(Suit.Paper, 4),
            new Card(Suit.Paper, 5),
            new Card(Suit.Scissors, 1),
            new Card(Suit.Scissors, 2),
            new Card(Suit.Scissors, 2),
            new Card(Suit.Scissors, 3)
        };

        public static readonly IReadOnlyList<Card> Contrario = new[]
        {
            new Card(Suit.Rock, 2),
            new Card(Suit.Rock, 3),
            new Card(Suit.Rock, 4),
            new Card(Suit.Paper, 1),
            new Card(Suit.Paper, 2),
            new Card(Suit.Paper, 3),
            new Card(Suit.Scissors, 3),
            new Card(Suit.Scissors, 4),
            new Card(Suit.Scissors, 5)
        };

        public static readonly IReadOnlyList<Card> EstatuaDePedra = new[]
        {
            new Card(Suit.Rock, 2),
            new Card(Suit.Rock, 3),
            new Card(Suit.Paper, 1),
            new Card(Suit.Rock, 4),
            new Card(Suit.Scissors, 2)
        };

        public static readonly IReadOnlyList<Card> Mumia = new[]
        {
            new Card(Suit.Scissors, 3),
            new Card(Suit.Paper, 2),
            new Card(Suit.Scissors, 4),
            new Card(Suit.Rock, 1),
            new Card(Suit.Scissors, 5)
        };

        public static readonly IReadOnlyList<Card> Pirata = new[]
        {
            new Card(Suit.Paper, 4),
            new Card(Suit.Rock, 5),
            new Card(Suit.Scissors, 3),
            new Card(Suit.Paper, 3),
            new Card(Suit.Rock, 4)
        };

        public static readonly IReadOnlyList<NamedDeck> Decks = new[]
        {
            new NamedDeck("Equilibrado", Equilibrado),
            new NamedDeck("Agressivo", Agressivo),
            new NamedDeck("Contrário", Contrario)
        };

        public static readonly IReadOnlyList<NamedEnemy> Enemies = new[]
        {
            new NamedEnemy("Estátua de Pedra", EstatuaDePedra, EnemyBehavior.Defensive),
            new NamedEnemy("Múmia", Mumia, EnemyBehavior.Defensive),
            new NamedEnemy("Pirata", Pirata, EnemyBehavior.Aggressive)
        };

        public static IReadOnlyList<IReadOnlyList<Card>> EnemySequences
        {
            get
            {
                return new IReadOnlyList<Card>[]
                {
                    EstatuaDePedra,
                    Mumia,
                    Pirata
                };
            }
        }
    }
}
