using NUnit.Framework;
using RockPaperPistol.Core;

namespace RockPaperPistol.Tests
{
    public class CardComparerTests
    {
        [Test]
        public void Rock1_vs_Paper1_PaperWins_WithSuitBonus()
        {
            Card player = new Card(Suit.Rock, 1);
            Card enemy = new Card(Suit.Paper, 1);

            RoundResolution result = CardComparer.Compare(player, enemy);

            Assert.AreEqual(RoundOutcome.EnemyWin, result.Outcome);
            Assert.AreEqual(1, result.PlayerAdjusted);
            Assert.AreEqual(2, result.EnemyAdjusted);
            Assert.AreEqual(1, result.StakeAwarded);
        }

        [Test]
        public void Paper1_vs_Rock1_PaperWins_WithSuitBonus()
        {
            Card player = new Card(Suit.Paper, 1);
            Card enemy = new Card(Suit.Rock, 1);

            RoundResolution result = CardComparer.Compare(player, enemy);

            Assert.AreEqual(RoundOutcome.PlayerWin, result.Outcome);
            Assert.AreEqual(2, result.PlayerAdjusted);
            Assert.AreEqual(1, result.EnemyAdjusted);
        }

        [Test]
        public void Rock2_vs_Rock3_HigherNumberWins()
        {
            Card player = new Card(Suit.Rock, 2);
            Card enemy = new Card(Suit.Rock, 3);

            RoundResolution result = CardComparer.Compare(player, enemy);

            Assert.AreEqual(RoundOutcome.EnemyWin, result.Outcome);
            Assert.AreEqual(2, result.PlayerAdjusted);
            Assert.AreEqual(3, result.EnemyAdjusted);
        }

        [Test]
        public void Scissors2_vs_Scissors2_IsDraw()
        {
            Card player = new Card(Suit.Scissors, 2);
            Card enemy = new Card(Suit.Scissors, 2);

            RoundResolution result = CardComparer.Compare(player, enemy, stake: 1);

            Assert.AreEqual(RoundOutcome.Draw, result.Outcome);
            Assert.AreEqual(2, result.PlayerAdjusted);
            Assert.AreEqual(2, result.EnemyAdjusted);
            Assert.AreEqual(0, result.StakeAwarded);
        }

        [Test]
        public void Paper3_vs_Scissors1_PaperWins_DespiteSuitDisadvantage()
        {
            Card player = new Card(Suit.Paper, 3);
            Card enemy = new Card(Suit.Scissors, 1);

            RoundResolution result = CardComparer.Compare(player, enemy);

            Assert.AreEqual(RoundOutcome.PlayerWin, result.Outcome);
            Assert.AreEqual(3, result.PlayerAdjusted);
            Assert.AreEqual(2, result.EnemyAdjusted);
        }

        [Test]
        public void Paper3_vs_Scissors2_NumbersTieAfterBonus_ScissorsWinsOnSuit()
        {
            Card player = new Card(Suit.Paper, 3);
            Card enemy = new Card(Suit.Scissors, 2);

            RoundResolution result = CardComparer.Compare(player, enemy);

            Assert.AreEqual(RoundOutcome.EnemyWin, result.Outcome);
            Assert.AreEqual(3, result.PlayerAdjusted);
            Assert.AreEqual(3, result.EnemyAdjusted);
        }

        [Test]
        public void Compare_UsesProvidedStake_OnWin()
        {
            Card player = new Card(Suit.Rock, 5);
            Card enemy = new Card(Suit.Rock, 1);

            RoundResolution result = CardComparer.Compare(player, enemy, stake: 3);

            Assert.AreEqual(RoundOutcome.PlayerWin, result.Outcome);
            Assert.AreEqual(3, result.StakeAwarded);
        }
    }
}
