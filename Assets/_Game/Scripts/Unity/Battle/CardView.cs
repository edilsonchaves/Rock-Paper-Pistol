using RockPaperPistol.Core;
using RockPaperPistol.Data;
using TMPro;
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
        [SerializeField] private SpriteRenderer _outline;
        [SerializeField] private BoxCollider2D _collider;

        [SerializeField] private Sprite _defaultBackSprite;
        public void Setup(Transform parent, CardDefinition cardDefinition, bool isShowCard, int handIndex = 0)
        {
            transform.SetParent(parent);
            _card = cardDefinition;
            _body.sprite = isShowCard ? cardDefinition.CardImage : _defaultBackSprite;
            HandIndex = handIndex;
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
    }
}
