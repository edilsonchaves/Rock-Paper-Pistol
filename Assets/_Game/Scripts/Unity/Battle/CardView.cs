using RockPaperPistol.Core;
using RockPaperPistol.Utils;
using TMPro;
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
        private TextMeshPro _value;
        private TextMeshPro _label;
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
            _body.sprite = PlaceholderArt.CardFront(card, displaySuit);
            _back.sprite = PlaceholderArt.CardBack();
            _value.text = plusOne ? displayValue + "+1" : displayValue.ToString();
            _label.text = card.IsPistol ? ShortPistolName(card.Pistol) : Card.SuitName(displaySuit);
            SetRevealed(revealed);
        }

        public void SetRevealed(bool revealed)
        {
            _body.enabled = revealed;
            _back.enabled = !revealed;
            _value.gameObject.SetActive(revealed);
            _label.gameObject.SetActive(revealed);
        }

        public void SetHover(bool hover)
        {
            _outline.enabled = hover && Interactable;
            int order = hover ? 16 : 10;
            _body.sortingOrder = order;
            _back.sortingOrder = order;
            _outline.sortingOrder = order + 1;
            _value.sortingOrder = order + 2;
            _label.sortingOrder = order + 2;
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
            _value = CreateLabel("Value", new Vector3(-0.08f, 0.78f, 0f), 3.4f, TextAlignmentOptions.TopLeft);
            _label = CreateLabel("Name", new Vector3(0f, -0.82f, 0f), 2.3f, TextAlignmentOptions.Center);
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

        private TextMeshPro CreateLabel(string name, Vector3 localPosition, float fontSize, TextAlignmentOptions align)
        {
            GameObject child = new GameObject(name);
            child.transform.SetParent(transform, false);
            child.transform.localPosition = localPosition;
            TextMeshPro tmp = child.AddComponent<TextMeshPro>();
            tmp.fontSize = fontSize;
            tmp.alignment = align;
            tmp.color = Color.black;
            tmp.fontStyle = FontStyles.Bold;
            tmp.enableWordWrapping = false;
            tmp.overflowMode = TextOverflowModes.Overflow;
            tmp.rectTransform.sizeDelta = new Vector2(1.4f, 0.45f);
            tmp.sortingOrder = 12;
            return tmp;
        }

        private static string ShortPistolName(PistolId pistol)
        {
            switch (pistol)
            {
                case PistolId.Pistoleiro:
                    return "Pistola";
                case PistolId.Estatua:
                    return "Estátua";
                case PistolId.Mumia:
                    return "Múmia";
                case PistolId.Pirata:
                    return "Pirata";
                default:
                    return "Pistola";
            }
        }
    }
}
