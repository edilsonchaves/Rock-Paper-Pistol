using System;
using System.Collections.Generic;
using RockPaperPistol.Core;
using RockPaperPistol.Data;
using RockPaperPistol.Unity.Battle;
using RockPaperPistol.Utils;
using UnityEngine;

namespace RockPaperPistol.Player
{
    public class PlayerControl : MonoBehaviour
    {
        [SerializeField] private List<CardDefinition> _deck;

        [SerializeField] private PlayerHandCard _hand;

        [SerializeField] private DeckDefinition _currentPlayer;
        public void Setup(DeckDefinition data, Action<CardView, CardDefinition> cardSelected)
        {
            _currentPlayer = data;
            foreach(var card in data.Sequence.Shuffle())
            {
                _deck.Add(card);
            }

            List<Card> cards = new List<Card>();
            foreach(var card in _deck)
            {
                    if(card is CardNormalDefinition)
                    {
                        var normalDefinition = (CardNormalDefinition) card;
                        Card c = new Card(normalDefinition.Suit,normalDefinition.Value);
                        cards.Add(c);
                    }
            }

            _hand.ReceiveCards(cards, _deck, true, true, cardSelected);
        }

        public Sprite GetAvatarSprite()
        {
            return _currentPlayer.ImageRoundWinner;
        }
    }
}