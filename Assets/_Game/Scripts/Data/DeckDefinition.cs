using System.Collections.Generic;
using RockPaperPistol.Core;
using UnityEngine;

namespace RockPaperPistol.Data
{
    [CreateAssetMenu(fileName = "Deck", menuName = "Rock Paper Pistol/Baralho")]
    public sealed class DeckDefinition : ScriptableObject
    {
        public string DisplayName;
        public CardData[] Cards = new CardData[9];
        public CardDefinition[] CardsOficial;
        public IReadOnlyList<Card> ToCards()
        {
            Card[] result = new Card[Cards != null ? Cards.Length : 0];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Cards[i].ToCard();
            }

            return result;
        }

        public NamedDeck ToNamedDeck()
        {
            return new NamedDeck(string.IsNullOrEmpty(DisplayName) ? name : DisplayName, ToCards());
        }
    }
}
