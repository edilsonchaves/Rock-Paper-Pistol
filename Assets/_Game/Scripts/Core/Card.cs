using System;

namespace RockPaperPistol.Core
{
    public enum CardKind
    {
        Basic = 0,
        Pistol = 1
    }

    public enum PistolId
    {
        None = 0,
        Pistoleiro = 1,
        Estatua = 2,
        Mumia = 3,
        Pirata = 4
    }

    public readonly struct Card : IEquatable<Card>
    {
        public const int MinValue = 1;
        public const int MaxValue = 5;

        public Suit Suit { get; }
        public int Value { get; }
        public CardKind Kind { get; }
        public PistolId Pistol { get; }

        public bool IsPistol => Kind == CardKind.Pistol && Pistol != PistolId.None;

        public Card(Suit suit, int value)
            : this(suit, value, CardKind.Basic, PistolId.None)
        {
        }

        public Card(Suit suit, int value, CardKind kind, PistolId pistol)
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
            Kind = kind;
            Pistol = pistol;
        }

        public static Card CreatePistol(PistolId pistol)
        {
            switch (pistol)
            {
                case PistolId.Pistoleiro:
                    return new Card(Suit.Rock, 1, CardKind.Pistol, PistolId.Pistoleiro);
                case PistolId.Estatua:
                    return new Card(Suit.Rock, 2, CardKind.Pistol, PistolId.Estatua);
                case PistolId.Mumia:
                    return new Card(Suit.Paper, 1, CardKind.Pistol, PistolId.Mumia);
                case PistolId.Pirata:
                    return new Card(Suit.Scissors, 1, CardKind.Pistol, PistolId.Pirata);
                default:
                    throw new ArgumentOutOfRangeException(nameof(pistol), pistol, "Pistola inválida.");
            }
        }

        public bool Equals(Card other) =>
            Suit == other.Suit
            && Value == other.Value
            && Kind == other.Kind
            && Pistol == other.Pistol;

        public override bool Equals(object obj) => obj is Card other && Equals(other);

        public override int GetHashCode() =>
            ((int)Suit * 31) ^ Value ^ ((int)Kind * 17) ^ ((int)Pistol * 13);

        public override string ToString()
        {
            if (IsPistol)
            {
                return PistolName(Pistol);
            }

            return $"{SuitName(Suit)} {Value}";
        }

        public static bool operator ==(Card left, Card right) => left.Equals(right);

        public static bool operator !=(Card left, Card right) => !left.Equals(right);

        public static string PistolName(PistolId pistol)
        {
            switch (pistol)
            {
                case PistolId.Pistoleiro:
                    return "Pistola do Pistoleiro";
                case PistolId.Estatua:
                    return "Pistola da Estátua";
                case PistolId.Mumia:
                    return "Pistola da Múmia";
                case PistolId.Pirata:
                    return "Pistola do Pirata";
                default:
                    return "Pistola";
            }
        }

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
