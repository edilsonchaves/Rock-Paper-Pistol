using System.Collections.Generic;
using NUnit.Framework;
using RockPaperPistol.Core;

namespace RockPaperPistol.Tests
{
    public class EnemyCardPickerTests
    {
        [Test]
        public void PathA_WhenTendencyHits_PicksOnlyAmongPreferredSuit()
        {
            IReadOnlyList<Card> hand = new[]
            {
                new Card(Suit.Paper, 1),
                new Card(Suit.Rock, 1),
                new Card(Suit.Rock, 3),
                Card.CreatePistol(PistolId.Estatua)
            };

            int chosen = EnemyCardPicker.ChooseHandIndex(
                hand,
                Suit.Rock,
                currentTurn: 1,
                maxTurns: 7,
                pistolAvailableFromTurn: 1,
                tendencyRoll: 0.10,
                nextIndex: count => 0);

            Assert.AreEqual(1, chosen);
            Assert.AreEqual(Suit.Rock, hand[chosen].Suit);
        }

        [Test]
        public void PathA_PreferredPoolIncludesPistolOfThatSuit()
        {
            IReadOnlyList<Card> hand = new[]
            {
                new Card(Suit.Paper, 2),
                Card.CreatePistol(PistolId.Estatua)
            };

            int chosen = EnemyCardPicker.ChooseHandIndex(
                hand,
                Suit.Rock,
                currentTurn: 1,
                maxTurns: 7,
                pistolAvailableFromTurn: 1,
                tendencyRoll: 0.20,
                nextIndex: count => 0);

            Assert.AreEqual(1, chosen);
            Assert.IsTrue(hand[chosen].IsPistol);
        }

        [Test]
        public void WhenTendencyMisses_PicksFromAllPlayableCards()
        {
            IReadOnlyList<Card> hand = new[]
            {
                new Card(Suit.Paper, 1),
                new Card(Suit.Rock, 3)
            };

            int chosen = EnemyCardPicker.ChooseHandIndex(
                hand,
                Suit.Rock,
                currentTurn: 1,
                maxTurns: 7,
                pistolAvailableFromTurn: 1,
                tendencyRoll: 0.80,
                nextIndex: count => 0);

            Assert.AreEqual(0, chosen);
        }

        [Test]
        public void MumiaPistol_IsLockedBeforeTurnThree()
        {
            IReadOnlyList<Card> hand = new[]
            {
                new Card(Suit.Paper, 2),
                Card.CreatePistol(PistolId.Mumia)
            };

            int chosen = EnemyCardPicker.ChooseHandIndex(
                hand,
                Suit.Paper,
                currentTurn: 2,
                maxTurns: 7,
                pistolAvailableFromTurn: 3,
                tendencyRoll: 0.10,
                nextIndex: count => 0);

            Assert.AreEqual(0, chosen);
            Assert.IsFalse(hand[chosen].IsPistol);
        }

        [Test]
        public void LastTurn_ForcesRemainingPistol()
        {
            IReadOnlyList<Card> hand = new[]
            {
                new Card(Suit.Scissors, 3),
                Card.CreatePistol(PistolId.Pirata)
            };

            int chosen = EnemyCardPicker.ChooseHandIndex(
                hand,
                Suit.Scissors,
                currentTurn: 7,
                maxTurns: 7,
                pistolAvailableFromTurn: 1,
                tendencyRoll: 0.10,
                nextIndex: count => 0);

            Assert.AreEqual(1, chosen);
            Assert.AreEqual(PistolId.Pirata, hand[chosen].Pistol);
        }
    }
}
