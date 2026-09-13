using RockPaperPistol.Core;

namespace RockPaperPistol.Unity.Battle
{
    public readonly struct HandPlay
    {
        public HandPlay(int handIndex, Card card, Suit suit)
        {
            HandIndex = handIndex;
            Card = card;
            Suit = suit;
        }

        public int HandIndex { get; }
        public Card Card { get; }
        public Suit Suit { get; }
    }
}
