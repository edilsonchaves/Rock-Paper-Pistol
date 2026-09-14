using System;
using System.Collections.Generic;
using RockPaperPistol.Core;
using RockPaperPistol.Data;
using RockPaperPistol.Utils;
using UnityEngine;

namespace RockPaperPistol.Unity.Battle
{
    public sealed class PlayerHandCard : MonoBehaviour
    {
        [SerializeField] private CardView _cardPrefab;
        [SerializeField] private Transform _hands;
        [SerializeField] private Transform _initialCardSpawnPosition;

        [SerializeField] private float _xDif;

        private readonly List<Card> _cards = new List<Card>();
        private readonly List<CardView> _views = new List<CardView>();
        private CardView _hover;
        private Suit? _chosenSuit;

        public IReadOnlyList<Card> Cards => _cards;
        public int Count => _cards.Count;
        public HandPlay? LastPlay { get; private set; }

        public void ReceiveCards(List<Card> cards, List<CardDefinition> cardDefinition, bool showHand, bool interactable, Action<CardView, CardDefinition> cardSelection)
        {
            _cards.Clear();
            if (cards != null)
            {
                for (int i = 0; i < cards.Count; i++)
                {
                    _cards.Add(cards[i]);
                    var cardView = Instantiate(_cardPrefab, Vector3.zero, Quaternion.identity);
                    cardView.Setup(_hands, cardDefinition[i], showHand, cardSelection);
                    _views.Add(cardView);
                }
            }
            Fan();
        }

        public void ChooseSuit(Suit suit)
        {
            _chosenSuit = suit;
        }

        public bool TrySelect(out HandPlay play)
        {
            play = default;
            CardView hit = RaycastCard();
            if (_hover != hit)
            {
                if (_hover != null)
                {
                    _hover.SetHover(false);
                }

                _hover = hit;
                if (_hover != null)
                {
                    _hover.SetHover(true);
                }
            }

            if (hit == null || !GameInput.LeftClickPressed || !hit.Interactable)
            {
                return false;
            }

            int index = hit.HandIndex;
            if (index < 0 || index >= _cards.Count)
            {
                return false;
            }

            Card card = _cards[index];
            Suit suit = _chosenSuit ?? card.Suit;
            play = new HandPlay(index, card, suit);
            LastPlay = play;
            _chosenSuit = null;
            return true;
        }

        private void Rebuild(bool interactable)
        {
            ClearViews();
            for (int i = 0; i < _cards.Count; i++)
            {
                Card card = _cards[i];
                CardView view = _views[i];
                view.HandIndex = i;
                view.Interactable = interactable;
                view.Bind(card, card.Suit, card.Value, false, true);
                view.SetColliderEnabled(true);
                _views.Add(view);
            }

            Fan();
        }

        private void Fan()
        {
            int count = _views.Count;
            for (int i = 0; i < count; i++)
            {
                _views[i].transform.localPosition = new Vector3(_initialCardSpawnPosition.position.x + _xDif * i, 0, 0f);
                _views[i].transform.localScale = Vector3.one * 0.38f;
            }
        }

        private void ClearViews()
        {
            _hover = null;
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

        private CardView RaycastCard()
        {
            Camera camera = Camera.main;
            if (camera == null)
            {
                return null;
            }

            Vector3 mouse = GameInput.MouseScreenPosition;
            mouse.z = Mathf.Abs(camera.transform.position.z);
            Vector3 world = camera.ScreenToWorldPoint(mouse);
            RaycastHit2D hit = Physics2D.Raycast(world, Vector2.zero);
            if (hit.collider == null)
            {
                return null;
            }

            CardView view = hit.collider.GetComponent<CardView>();
            return view != null && _views.Contains(view) ? view : null;
        }

        private static int DisplayValue(Card card, int turn)
        {
            if (card.Pistol == PistolId.Mumia)
            {
                return CardComparer.MumiaValueForTurn(turn);
            }

            return card.Value;
        }
    }
}
