using System.Collections.Generic;
using RockPaperPistol.Utils;

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
            : this(name, sequence, behavior, Suit.Rock, null, EnemyCardPicker.DefaultPistolAvailableFromTurn)
        {
        }

        public NamedEnemy(
            string name,
            IReadOnlyList<Card> sequence,
            EnemyBehavior behavior,
            Suit preferredSuit,
            Card? pistol,
            int pistolAvailableFromTurn = EnemyCardPicker.DefaultPistolAvailableFromTurn)
        {
            Name = name;
            Sequence = sequence;
            Behavior = behavior;
            PreferredSuit = preferredSuit;
            Pistol = pistol;
            PistolAvailableFromTurn = pistolAvailableFromTurn;
        }

        public string Name { get; }
        public IReadOnlyList<Card> Sequence { get; }
        public EnemyBehavior Behavior { get; }
        public Suit PreferredSuit { get; }
        public Card? Pistol { get; }
        public int PistolAvailableFromTurn { get; }
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
            new NamedEnemy(
                "Estátua de Pedra",
                BaseDeck,
                EnemyBehavior.Defensive,
                Suit.Rock,
                Card.CreatePistol(PistolId.Estatua)),
            new NamedEnemy(
                "Múmia",
                BaseDeck,
                EnemyBehavior.Defensive,
                Suit.Paper,
                Card.CreatePistol(PistolId.Mumia),
                pistolAvailableFromTurn: 3),
            new NamedEnemy(
                "Pirata",
                BaseDeck,
                EnemyBehavior.Aggressive,
                Suit.Scissors,
                Card.CreatePistol(PistolId.Pirata))
        };

        public static Card PlayerPistol => Card.CreatePistol(PistolId.Pistoleiro);

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
