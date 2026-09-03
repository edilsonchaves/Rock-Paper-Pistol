using System;
using RockPaperPistol.Core;
using UnityEngine;

namespace RockPaperPistol.Data
{
    [Serializable]
    public struct CardData
    {
        public Suit Suit;
        [Range(Card.MinValue, Card.MaxValue)]
        public int Value;

        public CardData(Suit suit, int value)
        {
            Suit = suit;
            Value = value;
        }

        public Card ToCard()
        {
            return new Card(Suit, Value);
        }
    }
}
