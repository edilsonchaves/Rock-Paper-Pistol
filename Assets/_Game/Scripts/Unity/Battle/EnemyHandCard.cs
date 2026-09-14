using System;
using System.Collections.Generic;
using UnityEngine;
using RockPaperPistol.Core;
using RockPaperPistol.Data;
using RockPaperPistol.Utils;

namespace RockPaperPistol.Unity.Battle
{
    public sealed class EnemyHandCard : MonoBehaviour
    {
        [SerializeField] private CardView _cardPrefab;
        private readonly List<Card> _cards = new List<Card>();
        private readonly List<CardView> _views = new List<CardView>();
        private readonly System.Random _rng = new System.Random();
        [SerializeField] private Transform _enemyHands;
        public IReadOnlyList<Card> Cards => _cards;
        public int Count => _cards.Count;
        public HandPlay? LastPlay { get; private set; }

        public void ReceiveCards(List<Card> cards, List<CardDefinition> cardDefinition, bool showHand)
        {
            _cards.Clear();
            if (cards != null)
            {
                for (int i = 0; i < cards.Count; i++)
                {
                    _cards.Add(cards[i]);
                    var cardView = Instantiate(_cardPrefab, Vector3.zero, Quaternion.identity);
                    cardView.Setup(_enemyHands, cardDefinition[i], showHand);
                    _views.Add(cardView);
                }
            }

            Fan();
        }

        public HandPlay ChoosePlay(Suit preferredSuit, int currentTurn, int maxTurns, int pistolAvailableFromTurn)
        {
            if (_cards.Count == 0)
            {
                throw new InvalidOperationException("O inimigo não tem cartas para jogar.");
            }

            int index = EnemyCardPicker.ChooseHandIndex(
                _cards,
                preferredSuit,
                currentTurn,
                maxTurns,
                pistolAvailableFromTurn,
                _rng);
            Card card = _cards[index];
            HandPlay play = new HandPlay(index, card, card.Suit);
            LastPlay = play;
            return play;
        }

        private void Rebuild(bool showHand)
        {
            ClearViews();
            if (!showHand)
            {
                return;
            }

            for (int i = 0; i < _cards.Count; i++)
            {
                Card card = _cards[i];
                CardView view = _views[i];
                view.HandIndex = i;
                view.Interactable = false;
                view.Bind(card, card.Suit, card.Value, false, false);
                view.SetColliderEnabled(false);
                _views.Add(view);
            }

            Fan();
        }

        private void Fan()
        {
            int count = _views.Count;
            for (int i = 0; i < count; i++)
            {
                float t = count <= 1 ? 0.5f : i / (float)(count - 1);
                float x = Mathf.Lerp(-1.6f, 1.6f, t);
                _views[i].transform.localPosition = new Vector3(x, 1.55f, 0f);
                _views[i].transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(10f, -10f, t));
                _views[i].transform.localScale = Vector3.one * 0.38f;
            }
        }

        private void ClearViews()
        {
            for (int i = 0; i < _views.Count; i++)
            {
                if (_views[i] != null)
                {
                    Destroy(_views[i].gameObject);
                }
            }

            _views.Clear();
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}
