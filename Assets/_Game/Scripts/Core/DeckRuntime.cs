using System;
using System.Collections.Generic;

namespace RockPaperPistol.Core
{
    public sealed class DeckRuntime
    {
        public const int DefaultHandSize = 3;

        private readonly List<Card> _drawPile = new List<Card>();
        private readonly List<Card> _hand = new List<Card>();
        private readonly List<Card> _discard = new List<Card>();
        private readonly List<Card> _composition = new List<Card>();
        private Random _rng = new Random();

        public IReadOnlyList<Card> DrawPile => _drawPile;
        public IReadOnlyList<Card> Hand => _hand;
        public IReadOnlyList<Card> Discard => _discard;
        public IReadOnlyList<Card> Composition => _composition;

        public int HandCount => _hand.Count;
        public int DrawCount => _drawPile.Count;
        public int DiscardCount => _discard.Count;

        public void ResetFrom(IReadOnlyList<Card> composition)
        {
            if (composition == null)
            {
                throw new ArgumentNullException(nameof(composition));
            }

            Card[] snapshot = new Card[composition.Count];
            for (int i = 0; i < composition.Count; i++)
            {
                snapshot[i] = composition[i];
            }

            _composition.Clear();
            _composition.AddRange(snapshot);

            _drawPile.Clear();
            _drawPile.AddRange(_composition);
            _hand.Clear();
            _discard.Clear();
        }

        public void SetRandom(Random rng)
        {
            _rng = rng ?? throw new ArgumentNullException(nameof(rng));
        }

        public void Shuffle()
        {
            for (int i = _drawPile.Count - 1; i > 0; i--)
            {
                int j = _rng.Next(i + 1);
                Card temp = _drawPile[i];
                _drawPile[i] = _drawPile[j];
                _drawPile[j] = temp;
            }
        }

        public void DrawUpTo(int handSize)
        {
            if (handSize < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(handSize));
            }

            while (_hand.Count < handSize && _drawPile.Count > 0)
            {
                int last = _drawPile.Count - 1;
                _hand.Add(_drawPile[last]);
                _drawPile.RemoveAt(last);
            }
        }

        public Card Play(int handIndex)
        {
            if (handIndex < 0 || handIndex >= _hand.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(handIndex));
            }

            Card played = _hand[handIndex];
            _hand.RemoveAt(handIndex);
            _discard.Add(played);
            return played;
        }

        public void PrepareEncounter(int handSize = DefaultHandSize)
        {
            ResetFrom(_composition);
            Shuffle();
            DrawUpTo(handSize);
        }
    }
}
