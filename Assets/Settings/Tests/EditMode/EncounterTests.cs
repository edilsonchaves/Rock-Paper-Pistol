using System.Collections.Generic;
using NUnit.Framework;
using RockPaperPistol.Core;

namespace RockPaperPistol.Tests
{
    public class EncounterTests
    {
        [Test]
        public void DrawThenWin_AwardsStackedStake()
        {
            Encounter encounter = new Encounter(Encounter.DefaultMaxTurns);

            EncounterRoundResult draw = encounter.PlayRound(
                new Card(Suit.Scissors, 2),
                new Card(Suit.Scissors, 2));
            Assert.AreEqual(RoundOutcome.Draw, draw.Resolution.Outcome);
            Assert.AreEqual(0, draw.PlayerScore);
            Assert.AreEqual(0, draw.EnemyScore);
            Assert.AreEqual(2, draw.NextStake);

            EncounterRoundResult win = encounter.PlayRound(
                new Card(Suit.Rock, 3),
                new Card(Suit.Rock, 1));
            Assert.AreEqual(RoundOutcome.PlayerWin, win.Resolution.Outcome);
            Assert.AreEqual(2, win.Resolution.StakeAwarded);
            Assert.AreEqual(2, win.PlayerScore);
            Assert.AreEqual(1, win.NextStake);
        }

        [Test]
        public void TwoDrawsThenWin_AwardsStakeOfThree()
        {
            Encounter encounter = new Encounter(Encounter.DefaultMaxTurns);

            encounter.PlayRound(new Card(Suit.Paper, 3), new Card(Suit.Paper, 3));
            encounter.PlayRound(new Card(Suit.Paper, 3), new Card(Suit.Paper, 3));
            EncounterRoundResult win = encounter.PlayRound(
                new Card(Suit.Paper, 3),
                new Card(Suit.Rock, 1));

            Assert.AreEqual(3, win.Resolution.StakeAwarded);
            Assert.AreEqual(3, win.PlayerScore);
        }

        [Test]
        public void AfterMaxTurns_HigherPlayerScore_WinsEncounter()
        {
            Encounter encounter = new Encounter(Encounter.DefaultMaxTurns);
            for (int i = 0; i < Encounter.DefaultMaxTurns; i++)
            {
                encounter.PlayRound(new Card(Suit.Rock, 3), new Card(Suit.Rock, 1));
            }

            Assert.IsTrue(encounter.IsFinished);
            Assert.AreEqual(EncounterStatus.PlayerWon, encounter.Status);
            Assert.AreEqual(Encounter.DefaultMaxTurns, encounter.PlayerScore);
            Assert.AreEqual(0, encounter.EnemyScore);
            Assert.AreEqual(7, encounter.MaxTurns);
        }

        [Test]
        public void TiedScore_IsPlayerLoss()
        {
            Encounter encounter = new Encounter(3);

            encounter.PlayRound(new Card(Suit.Rock, 3), new Card(Suit.Rock, 1));
            encounter.PlayRound(new Card(Suit.Rock, 1), new Card(Suit.Rock, 3));
            encounter.PlayRound(new Card(Suit.Rock, 2), new Card(Suit.Rock, 2));

            Assert.AreEqual(1, encounter.PlayerScore);
            Assert.AreEqual(1, encounter.EnemyScore);
            Assert.AreEqual(EncounterStatus.PlayerLost, encounter.Status);
        }

        [Test]
        public void MaxTurns_IsConfigurable()
        {
            Encounter shortEncounter = new Encounter(2);
            shortEncounter.PlayRound(new Card(Suit.Rock, 3), new Card(Suit.Rock, 1));
            Assert.IsFalse(shortEncounter.IsFinished);
            shortEncounter.PlayRound(new Card(Suit.Rock, 3), new Card(Suit.Rock, 1));
            Assert.IsTrue(shortEncounter.IsFinished);
            Assert.AreEqual(2, shortEncounter.MaxTurns);
        }

        [Test]
        public void PlayerUsingCard_DoesNotRemoveEnemyCopy()
        {
            RunSession run = new RunSession(ThreeWithBaseDeck());
            run.SelectDeck(DefaultCatalog.BaseDeck, new System.Random(1));

            Card pedra3 = new Card(Suit.Rock, 3);
            int playerIndex = IndexOf(run.Deck.Hand, pedra3);
            Assert.GreaterOrEqual(playerIndex, 0);
            Assert.IsTrue(ContainsCard(run.EnemyDeck.Hand, pedra3));

            run.PlayFromHand(playerIndex);

            Assert.IsFalse(ContainsCard(run.Deck.Hand, pedra3));
            Assert.IsTrue(ContainsCard(run.Deck.Discard, pedra3));
            Assert.IsTrue(ContainsCard(run.EnemyDeck.Hand, pedra3));
        }

        [Test]
        public void RunSession_StartsWithNineAvailableCardsEach()
        {
            RunSession run = new RunSession(ThreeWithBaseDeck());
            run.SelectDeck(DefaultCatalog.BaseDeck, new System.Random(1));

            Assert.AreEqual(9, run.Deck.HandCount);
            Assert.AreEqual(9, run.EnemyDeck.HandCount);
            Assert.AreEqual(Encounter.DefaultMaxTurns, run.MaxTurns);
            CollectionAssert.AreEqual(DefaultCatalog.BaseDeck, run.Deck.Composition);
            CollectionAssert.AreEqual(DefaultCatalog.BaseDeck, run.EnemyDeck.Composition);
        }

        [Test]
        public void RunSession_VictoryResetsDeckForNextEnemy()
        {
            IReadOnlyList<NamedEnemy> easyEnemies = new[]
            {
                new NamedEnemy("A", NineWeak()),
                new NamedEnemy("B", NineWeak()),
                new NamedEnemy("C", NineWeak())
            };

            RunSession run = new RunSession(easyEnemies);
            run.SelectDeck(NineStrong(), new System.Random(1));

            PlayUntilEncounterEnds(run);
            Assert.AreEqual(RunPhase.InEncounter, run.Phase);
            Assert.AreEqual(1, run.EnemyIndex);
            Assert.AreEqual(1, run.EnemiesDefeated);
            Assert.AreEqual(0, run.Deck.DiscardCount);
            Assert.AreEqual(9, run.Deck.HandCount + run.Deck.DrawCount);
            Assert.AreEqual(9, run.Deck.HandCount);
            Assert.AreEqual(9, run.EnemyDeck.HandCount);
        }

        [Test]
        public void RunSession_Loss_GoesToGameOver()
        {
            IReadOnlyList<NamedEnemy> hard = new[]
            {
                new NamedEnemy("A", SevenHigh()),
                new NamedEnemy("B", SevenHigh()),
                new NamedEnemy("C", SevenHigh())
            };

            RunSession run = new RunSession(hard);
            run.SelectDeck(NineWeak(), new System.Random(1));
            PlayUntilEncounterEnds(run);

            Assert.AreEqual(RunPhase.GameOver, run.Phase);
            Assert.AreEqual(0, run.EnemiesDefeated);
        }

        [Test]
        public void RunSession_ThreeWins_IsVictory()
        {
            IReadOnlyList<NamedEnemy> easyEnemies = new[]
            {
                new NamedEnemy("A", NineWeak()),
                new NamedEnemy("B", NineWeak()),
                new NamedEnemy("C", NineWeak())
            };

            RunSession run = new RunSession(easyEnemies);
            run.SelectDeck(NineStrong(), new System.Random(7));
            PlayUntilEncounterEnds(run);
            PlayUntilEncounterEnds(run);
            PlayUntilEncounterEnds(run);

            Assert.AreEqual(RunPhase.Victory, run.Phase);
            Assert.AreEqual(3, run.EnemiesDefeated);
        }

        private static IReadOnlyList<NamedEnemy> ThreeWithBaseDeck()
        {
            return new[]
            {
                new NamedEnemy("A", DefaultCatalog.BaseDeck),
                new NamedEnemy("B", DefaultCatalog.BaseDeck),
                new NamedEnemy("C", DefaultCatalog.BaseDeck)
            };
        }

        private static IReadOnlyList<Card> NineStrong()
        {
            return new[]
            {
                new Card(Suit.Paper, 3),
                new Card(Suit.Paper, 3),
                new Card(Suit.Paper, 3),
                new Card(Suit.Paper, 3),
                new Card(Suit.Paper, 3),
                new Card(Suit.Paper, 3),
                new Card(Suit.Paper, 3),
                new Card(Suit.Paper, 3),
                new Card(Suit.Paper, 3)
            };
        }

        private static IReadOnlyList<Card> NineWeak()
        {
            return new[]
            {
                new Card(Suit.Rock, 1),
                new Card(Suit.Rock, 1),
                new Card(Suit.Rock, 1),
                new Card(Suit.Rock, 1),
                new Card(Suit.Rock, 1),
                new Card(Suit.Rock, 1),
                new Card(Suit.Rock, 1),
                new Card(Suit.Rock, 1),
                new Card(Suit.Rock, 1)
            };
        }

        private static IReadOnlyList<Card> SevenHigh()
        {
            return new[]
            {
                new Card(Suit.Rock, 3),
                new Card(Suit.Rock, 3),
                new Card(Suit.Rock, 3),
                new Card(Suit.Rock, 3),
                new Card(Suit.Rock, 3),
                new Card(Suit.Rock, 3),
                new Card(Suit.Rock, 3)
            };
        }

        private static void PlayUntilEncounterEnds(RunSession run)
        {
            Encounter encounter = run.CurrentEncounter;
            int guard = 0;
            while (encounter != null && !encounter.IsFinished)
            {
                run.PlayFromHand(0);
                if (++guard > 20)
                {
                    Assert.Fail("Loop infinito no encontro.");
                }
            }
        }

        private static int IndexOf(IReadOnlyList<Card> cards, Card target)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].Equals(target))
                {
                    return i;
                }
            }

            return -1;
        }

        private static bool ContainsCard(IReadOnlyList<Card> cards, Card target)
        {
            return IndexOf(cards, target) >= 0;
        }
    }
}
