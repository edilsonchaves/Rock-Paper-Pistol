using System;
using System.Collections.Generic;
using NUnit.Framework;
using RockPaperPistol.Core;

namespace RockPaperPistol.Tests
{
    public class DeckRuntimeTests
    {
        private static IReadOnlyList<Card> NineCards()
        {
            return DefaultCatalog.BaseDeck;
        }

        [Test]
        public void ResetFrom_ThenDrawUpTo_FillsAllNineCardsInHand()
        {
            DeckRuntime deck = new DeckRuntime();
            deck.ResetFrom(NineCards());
            deck.DrawUpTo(DeckRuntime.DefaultHandSize);

            Assert.AreEqual(9, deck.HandCount);
            Assert.AreEqual(0, deck.DrawCount);
            Assert.AreEqual(0, deck.DiscardCount);
            Assert.AreEqual(9, deck.Composition.Count);
        }

        [Test]
        public void Play_MovesCardFromHandToDiscard()
        {
            DeckRuntime deck = new DeckRuntime();
            deck.ResetFrom(NineCards());
            deck.DrawUpTo(DeckRuntime.DefaultHandSize);
            Card expected = deck.Hand[1];

            Card played = deck.Play(1);

            Assert.AreEqual(expected, played);
            Assert.AreEqual(8, deck.HandCount);
            Assert.AreEqual(1, deck.DiscardCount);
            Assert.AreEqual(played, deck.Discard[0]);
            Assert.IsFalse(ContainsCard(deck.Hand, played));
        }

        [Test]
        public void DrawUpTo_AfterPlay_DoesNotReturnDiscardedCard()
        {
            DeckRuntime deck = new DeckRuntime();
            deck.ResetFrom(NineCards());
            deck.DrawUpTo(DeckRuntime.DefaultHandSize);
            Card played = deck.Play(0);
            deck.DrawUpTo(DeckRuntime.DefaultHandSize);

            Assert.AreEqual(8, deck.HandCount);
            Assert.AreEqual(0, deck.DrawCount);
            Assert.AreEqual(1, deck.DiscardCount);
            Assert.IsFalse(ContainsCard(deck.Hand, played));
        }

        [Test]
        public void DiscardedCards_DoNotReturnUntilReset()
        {
            DeckRuntime deck = new DeckRuntime();
            deck.ResetFrom(NineCards());
            deck.DrawUpTo(DeckRuntime.DefaultHandSize);

            Card first = deck.Play(0);
            deck.DrawUpTo(DeckRuntime.DefaultHandSize);
            Card second = deck.Play(0);

            Assert.AreEqual(2, deck.DiscardCount);
            Assert.IsFalse(ContainsCard(deck.Hand, first));
            Assert.IsFalse(ContainsCard(deck.DrawPile, first));
            Assert.IsTrue(ContainsCard(deck.Discard, first));
            Assert.IsTrue(ContainsCard(deck.Discard, second));
        }

        [Test]
        public void PrepareEncounter_ResetsFullDeckAndDrawsAllNine()
        {
            DeckRuntime deck = new DeckRuntime();
            deck.ResetFrom(NineCards());
            deck.DrawUpTo(DeckRuntime.DefaultHandSize);
            deck.Play(0);
            deck.Play(0);

            deck.PrepareEncounter();

            Assert.AreEqual(9, deck.HandCount);
            Assert.AreEqual(0, deck.DrawCount);
            Assert.AreEqual(0, deck.DiscardCount);
            Assert.AreEqual(9, deck.HandCount + deck.DrawCount + deck.DiscardCount);
        }

        [Test]
        public void PrepareMatchHand_DrawsEightBasicsAddsPistolAndExcludesTheRest()
        {
            DeckRuntime deck = new DeckRuntime();
            deck.ResetFrom(NineCards());
            deck.SetRandom(new Random(3));
            Card pistol = Card.CreatePistol(PistolId.Pistoleiro);

            deck.PrepareMatchHand(pistol);

            Assert.AreEqual(9, deck.HandCount);
            Assert.AreEqual(0, deck.DrawCount);
            Assert.AreEqual(1, deck.Excluded.Count);
            Assert.IsTrue(ContainsCard(deck.Hand, pistol));
            Assert.AreEqual(8, CountBasics(deck.Hand));
        }

        [Test]
        public void Shuffle_WithFixedSeed_IsDeterministic()
        {
            DeckRuntime a = new DeckRuntime();
            a.ResetFrom(NineCards());
            a.SetRandom(new Random(42));
            a.Shuffle();

            DeckRuntime b = new DeckRuntime();
            b.ResetFrom(NineCards());
            b.SetRandom(new Random(42));
            b.Shuffle();

            CollectionAssert.AreEqual(a.DrawPile, b.DrawPile);
        }

        private static int CountBasics(IReadOnlyList<Card> cards)
        {
            int count = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                if (!cards[i].IsPistol)
                {
                    count += 1;
                }
            }

            return count;
        }

        private static bool ContainsCard(IReadOnlyList<Card> cards, Card target)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].Equals(target))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
