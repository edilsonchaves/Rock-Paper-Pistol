using NUnit.Framework;
using RockPaperPistol.Core;

namespace RockPaperPistol.Tests
{
    public class PistolComparerTests
    {
        [Test]
        public void Pistoleiro_vs_Rock2_WinsAsPaper2()
        {
            RoundResolution result = CardComparer.Compare(
                Card.CreatePistol(PistolId.Pistoleiro),
                new Card(Suit.Rock, 2));

            Assert.AreEqual(Suit.Paper, result.PlayerSuit);
            Assert.AreEqual(2, result.PlayerAdjusted);
            Assert.AreEqual(2, result.EnemyAdjusted);
            Assert.AreEqual(RoundOutcome.PlayerWin, result.Outcome);
        }

        [Test]
        public void Pistoleiro_vs_Rock3_Loses()
        {
            RoundResolution result = CardComparer.Compare(
                Card.CreatePistol(PistolId.Pistoleiro),
                new Card(Suit.Rock, 3));

            Assert.AreEqual(2, result.PlayerAdjusted);
            Assert.AreEqual(3, result.EnemyAdjusted);
            Assert.AreEqual(RoundOutcome.EnemyWin, result.Outcome);
        }

        [Test]
        public void Pirata_vs_Paper_CapsAtThreeAndDoesNotReachFour()
        {
            RoundResolution result = CardComparer.Compare(
                Card.CreatePistol(PistolId.Pirata),
                new Card(Suit.Paper, 3));

            Assert.AreEqual(3, result.PlayerAdjusted);
            Assert.AreEqual(3, result.EnemyAdjusted);
            Assert.AreEqual(RoundOutcome.PlayerWin, result.Outcome);
        }

        [Test]
        public void Pirata_vs_Scissors3_IsDraw()
        {
            RoundResolution result = CardComparer.Compare(
                Card.CreatePistol(PistolId.Pirata),
                new Card(Suit.Scissors, 3));

            Assert.AreEqual(3, result.PlayerAdjusted);
            Assert.AreEqual(3, result.EnemyAdjusted);
            Assert.AreEqual(RoundOutcome.Draw, result.Outcome);
        }

        [Test]
        public void Pirata_vs_Rock_BehavesAsScissors1()
        {
            RoundResolution result = CardComparer.Compare(
                Card.CreatePistol(PistolId.Pirata),
                new Card(Suit.Rock, 1));

            Assert.AreEqual(1, result.PlayerAdjusted);
            Assert.AreEqual(2, result.EnemyAdjusted);
            Assert.AreEqual(RoundOutcome.EnemyWin, result.Outcome);
        }

        [Test]
        public void Mumia_Turn7_vs_Rock_ReachesFour()
        {
            RoundResolution result = CardComparer.Compare(
                Card.CreatePistol(PistolId.Mumia),
                new Card(Suit.Rock, 3),
                turn: 7);

            Assert.AreEqual(4, result.PlayerAdjusted);
            Assert.AreEqual(3, result.EnemyAdjusted);
            Assert.AreEqual(RoundOutcome.PlayerWin, result.Outcome);
        }

        [Test]
        public void Mumia_Turn1_HasValueOne()
        {
            RoundResolution result = CardComparer.Compare(
                Card.CreatePistol(PistolId.Mumia),
                new Card(Suit.Scissors, 1),
                turn: 1);

            Assert.AreEqual(1, result.PlayerAdjusted);
        }

        [Test]
        public void Estatua_vs_Scissors_IsThreeWithoutStacking()
        {
            RoundResolution result = CardComparer.Compare(
                Card.CreatePistol(PistolId.Estatua),
                new Card(Suit.Scissors, 2));

            Assert.AreEqual(3, result.PlayerAdjusted);
            Assert.AreEqual(RoundOutcome.PlayerWin, result.Outcome);
        }

        [Test]
        public void Estatua_vs_Rock_DropsToOne()
        {
            RoundResolution result = CardComparer.Compare(
                Card.CreatePistol(PistolId.Estatua),
                new Card(Suit.Rock, 2));

            Assert.AreEqual(1, result.PlayerAdjusted);
            Assert.AreEqual(2, result.EnemyAdjusted);
            Assert.AreEqual(RoundOutcome.EnemyWin, result.Outcome);
        }

        [Test]
        public void Pistoleiro_ResolvesBeforeEstatua()
        {
            RoundResolution result = CardComparer.Compare(
                Card.CreatePistol(PistolId.Pistoleiro),
                Card.CreatePistol(PistolId.Estatua));

            Assert.AreEqual(Suit.Paper, result.PlayerSuit);
            Assert.AreEqual(2, result.PlayerAdjusted);
            Assert.AreEqual(3, result.EnemyAdjusted);
            Assert.AreEqual(RoundOutcome.EnemyWin, result.Outcome);
        }
    }
}
