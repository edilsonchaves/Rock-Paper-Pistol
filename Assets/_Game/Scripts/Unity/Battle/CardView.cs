using System;
using RockPaperPistol.Core;
using RockPaperPistol.Data;
using UnityEngine;

namespace RockPaperPistol.Unity.Battle
{
    public sealed class CardView : MonoBehaviour
    {
        public int HandIndex { get; set; }
        public Card BoundCard { get; private set; }
        public bool Interactable { get; set; }

        [SerializeField] private CardDefinition _card;
        [SerializeField] private SpriteRenderer _body;
        [SerializeField] private SpriteRenderer _plusOne;
        [SerializeField] private SpriteRenderer _outline;
        [SerializeField] private BoxCollider2D _collider;

        [SerializeField] private Sprite _defaultBackSprite;

        private Action<CardView, CardDefinition> _cardSelection;
        public void Setup(Transform parent, CardDefinition cardDefinition, bool isShowCard, Action<CardView, CardDefinition> cardSelection, int handIndex = 0)
        {
            transform.SetParent(parent);
            _card = cardDefinition;
            _body.sprite = isShowCard ? cardDefinition.CardImage : _defaultBackSprite;
            HandIndex = handIndex;
            _cardSelection = cardSelection;
        }

        public void Bind(Card card, Suit displaySuit, int displayValue, bool plusOne, bool revealed)
        {
            BoundCard = card;
            _body.sprite = PlaceholderArt.CardFront(card, displaySuit);
            SetRevealed(revealed);
        }

        public void SetRevealed(bool revealed)
        {
            _body.enabled = revealed;
        }

        public void SetHover(bool hover)
        {
            _outline.enabled = hover && Interactable;
            int order = hover ? 16 : 10;
            _body.sortingOrder = order;
            _outline.sortingOrder = order + 1;
        }

        public void SetColliderEnabled(bool enabled)
        {
            if (_collider != null)
            {
                _collider.enabled = enabled && Interactable;
            }
        }

        public void EnemySelected()
        {
            Debug.Log("Testando");
            _body.sprite = _card.CardImage;
            _cardSelection?.Invoke(this, _card);
        }

        public void CardClicked()
        {
            _cardSelection?.Invoke(this, _card);
        }

        public int GetValue(int addValue = 0)
        {
            var result = 0;
            if(addValue > 0)
            {
                CardIsStronger();
            }

            if(_card is CardNormalDefinition)
            {
                var cardNormal = (CardNormalDefinition) _card;
                result = cardNormal.Value + addValue;
            }
            return result;
        }

        private void CardIsStronger()
        {
            _plusOne.enabled = true;
        }
    }
}
