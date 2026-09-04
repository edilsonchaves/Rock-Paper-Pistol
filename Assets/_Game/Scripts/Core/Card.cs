using System;

namespace RockPaperPistol.Core
{
    public readonly struct Card : IEquatable<Card>
    {
        public const int MinValue = 1;
        public const int MaxValue = 5;

        public Suit Suit { get; }
        public int Value { get; }

        public Card(Suit suit, int value)
        {
            if (value < MinValue || value > MaxValue)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    value,
                    $"O número da carta deve estar entre {MinValue} e {MaxValue}.");
            }

            Suit = suit;
            Value = value;
        }

        public bool Equals(Card other) => Suit == other.Suit && Value == other.Value;

        public override bool Equals(object obj) => obj is Card other && Equals(other);

        public override int GetHashCode() => ((int)Suit * 31) ^ Value;

        public override string ToString() => $"{SuitName(Suit)} {Value}";

        public static bool operator ==(Card left, Card right) => left.Equals(right);

        public static bool operator !=(Card left, Card right) => !left.Equals(right);

        public static string SuitName(Suit suit)
        {
            switch (suit)
            {
                case Suit.Rock:
                    return "Pedra";
                case Suit.Paper:
                    return "Papel";
                case Suit.Scissors:
                    return "Tesoura";
                default:
                    return suit.ToString();
            }
        }
    }
}
