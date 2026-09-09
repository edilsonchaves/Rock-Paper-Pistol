using RockPaperPistol.Core;
using UnityEngine;

namespace RockPaperPistol.Unity.Battle
{
    public sealed class CardView : MonoBehaviour
    {
        public int HandIndex { get; set; }
        public Card BoundCard { get; private set; }
        public bool Interactable { get; set; }

        private SpriteRenderer _body;
        private SpriteRenderer _outline;
        private SpriteRenderer _back;
        private BoxCollider2D _collider;

        public static CardView Create(Transform parent, string name)
        {
            GameObject root = new GameObject(name);
            root.transform.SetParent(parent, false);
            CardView view = root.AddComponent<CardView>();
            view.Build();
            return view;
        }

        public void Bind(Card card, Suit displaySuit, int displayValue, bool plusOne, bool revealed)
        {
            BoundCard = card;
            _body.sprite = PlaceholderArt.CardFront(card, displaySuit, displayValue, plusOne);
            _back.sprite = PlaceholderArt.CardBack();
            SetRevealed(revealed);
        }

        public void SetRevealed(bool revealed)
        {
            _body.enabled = revealed;
            _back.enabled = !revealed;
        }

        public void SetHover(bool hover)
        {
            _outline.enabled = hover && Interactable;
            int order = hover ? 16 : 10;
            _body.sortingOrder = order;
            _back.sortingOrder = order;
            _outline.sortingOrder = order + 1;
        }

        public void SetColliderEnabled(bool enabled)
        {
            if (_collider != null)
            {
                _collider.enabled = enabled && Interactable;
            }
        }

        private void Build()
        {
            _body = CreateLayer("Front", 10);
            _back = CreateLayer("Back", 10);
            _outline = CreateLayer("Outline", 11);
            _outline.sprite = PlaceholderArt.GoldOutline();
            _outline.enabled = false;
            _collider = gameObject.AddComponent<BoxCollider2D>();
            _collider.size = new Vector2(1.55f, 2.15f);
        }

        private SpriteRenderer CreateLayer(string name, int order)
        {
            GameObject child = new GameObject(name);
            child.transform.SetParent(transform, false);
            SpriteRenderer renderer = child.AddComponent<SpriteRenderer>();
            renderer.sortingOrder = order;
            PlaceholderArt.ApplyVisibleMaterial(renderer);
            return renderer;
        }
    }
}
