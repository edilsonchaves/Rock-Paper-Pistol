using UnityEngine;
using RockPaperPistol.Data;
using System.Collections.Generic;
using RockPaperPistol.Core;
using RockPaperPistol.Unity.Battle;
using RockPaperPistol.Utils;
using System;

namespace RockPaperPistol.Enemy
{ 
    public class EnemyControl : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _avatarBody;
        [SerializeField] private string _avatarName;
        [SerializeField] private List<CardDefinition> _enemyDeck;

        [SerializeField] private EnemyHandCard _enemyHand;

        [SerializeField] private EnemyDefinition _currentEnemy;
        public void SetupEnemy(EnemyDefinition data, Action<CardView, CardDefinition> cardSelected)
        {
            _currentEnemy = data;
            _avatarBody.color = data.SpriteColor;
            _avatarName = data.DisplayName;
            foreach(var card in data.Sequence.Shuffle())
            {
                _enemyDeck.Add(card);
            }
            List<Card> cards = new List<Card>();
            foreach(var card in _enemyDeck)
            {
                    if(card is CardNormalDefinition)
                    {
                        var normalDefinition = (CardNormalDefinition) card;
                        Card c = new Card(normalDefinition.Suit,normalDefinition.Value);
                        cards.Add(c);                  
                    }
            }

            _enemyHand.ReceiveCards(cards, _enemyDeck, false, cardSelected);
        }

        public void ThrowCardInSequence()
        {
            _enemyHand.ThrowSequence();
        }

        public Sprite GetAvatarSprite()
        {
            return _currentEnemy.ImageRoundWinner;
        }
    }
}