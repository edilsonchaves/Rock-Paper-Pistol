using System.Collections.Generic;
using NUnit.Framework;
using RockPaperPistol.Core;

namespace RockPaperPistol.Tests
{
    public class EncounterTests
    {
        private static Encounter FiveRocks(int value)
        {
            return new Encounter(new[]
            {
                new Card(Suit.Rock, value),
                new Card(Suit.Rock, value),
                new Card(Suit.Rock, value),
                new Card(Suit.Rock, value),
                new Card(Suit.Rock, value)
            });
        }

        [Test]
        public void DrawThenWin_AwardsStackedStake()
        {
            Encounter encounter = new Encounter(new[]
            {
                new Card(Suit.Scissors, 2),
                new Card(Suit.Rock, 1),
                new Card(Suit.Rock, 1),
                new Card(Suit.Rock, 1),
                new Card(Suit.Rock, 1)
            });

            EncounterRoundResult draw = encounter.PlayRound(new Card(Suit.Scissors, 2));
            Assert.AreEqual(RoundOutcome.Draw, draw.Resolution.Outcome);
            Assert.AreEqual(0, draw.PlayerScore);
            Assert.AreEqual(0, draw.EnemyScore);
            Assert.AreEqual(2, draw.NextStake);

            EncounterRoundResult win = encounter.PlayRound(new Card(Suit.Rock, 5));
            Assert.AreEqual(RoundOutcome.PlayerWin, win.Resolution.Outcome);
            Assert.AreEqual(2, win.Resolution.StakeAwarded);
            Assert.AreEqual(2, win.PlayerScore);
            Assert.AreEqual(1, win.NextStake);
        }

        [Test]
        public void TwoDrawsThenWin_AwardsStakeOfThree()
        {
            Encounter encounter = new Encounter(new[]
            {
                new Card(Suit.Paper, 3),
                new Card(Suit.Paper, 3),
                new Card(Suit.Rock, 1),
                new Card(Suit.Rock, 1),
                new Card(Suit.Rock, 1)
            });

            encounter.PlayRound(new Card(Suit.Paper, 3));
            encounter.PlayRound(new Card(Suit.Paper, 3));
            EncounterRoundResult win = encounter.PlayRound(new Card(Suit.Paper, 5));

            Assert.AreEqual(3, win.Resolution.StakeAwarded);
            Assert.AreEqual(3, win.PlayerScore);
        }

        [Test]
        public void AfterFiveRounds_HigherPlayerScore_WinsEncounter()
        {
            Encounter encounter = FiveRocks(1);
            for (int i = 0; i < Encounter.RoundsPerEncounter; i++)
            {
                encounter.PlayRound(new Card(Suit.Rock, 5));
            }

            Assert.IsTrue(encounter.IsFinished);
            Assert.AreEqual(EncounterStatus.PlayerWon, encounter.Status);
            Assert.AreEqual(5, encounter.PlayerScore);
            Assert.AreEqual(0, encounter.EnemyScore);
        }

        [Test]
        public void TiedScore_IsPlayerLoss()
        {
            Encounter encounter = new Encounter(new[]
            {
                new Card(Suit.Rock, 3),
                new Card(Suit.Rock, 3),
                new Card(Suit.Rock, 3),
                new Card(Suit.Rock, 3),
                new Card(Suit.Rock, 3)
            });

            encounter.PlayRound(new Card(Suit.Rock, 5));
            encounter.PlayRound(new Card(Suit.Rock, 1));
            encounter.PlayRound(new Card(Suit.Rock, 5));
            encounter.PlayRound(new Card(Suit.Rock, 1));
            encounter.PlayRound(new Card(Suit.Rock, 3));

            Assert.AreEqual(2, encounter.PlayerScore);
            Assert.AreEqual(2, encounter.EnemyScore);
            Assert.AreEqual(EncounterStatus.PlayerLost, encounter.Status);
        }

        [Test]
        public void RunSession_VictoryResetsDeckForNextEnemy()
        {
            IReadOnlyList<NamedEnemy> easyEnemies = new[]
            {
                new NamedEnemy("A", FiveLow()),
                new NamedEnemy("B", FiveLow()),
                new NamedEnemy("C", FiveLow())
            };

            RunSession run = new RunSession(easyEnemies);
            run.SelectDeck(DefaultCatalog.Equilibrado, new System.Random(1));

            PlayUntilEncounterEnds(run);
            Assert.AreEqual(RunPhase.InEncounter, run.Phase);
            Assert.AreEqual(1, run.EnemyIndex);
            Assert.AreEqual(1, run.EnemiesDefeated);
            Assert.AreEqual(0, run.Deck.DiscardCount);
            Assert.AreEqual(9, run.Deck.HandCount + run.Deck.DrawCount);
            Assert.AreEqual(3, run.Deck.HandCount);
        }

        [Test]
        public void RunSession_Loss_GoesToGameOver()
        {
            IReadOnlyList<NamedEnemy> hard = new[]
            {
                new NamedEnemy("A", FiveHigh()),
                new NamedEnemy("B", FiveHigh()),
                new NamedEnemy("C", FiveHigh())
            };

            RunSession run = new RunSession(hard);
            run.SelectDeck(DefaultCatalog.Equilibrado, new System.Random(1));
            PlayUntilEncounterEnds(run);

            Assert.AreEqual(RunPhase.GameOver, run.Phase);
            Assert.AreEqual(0, run.EnemiesDefeated);
        }

        [Test]
        public void RunSession_ThreeWins_IsVictory()
        {
            IReadOnlyList<NamedEnemy> easyEnemies = new[]
            {
                new NamedEnemy("A", FiveLow()),
                new NamedEnemy("B", FiveLow()),
                new NamedEnemy("C", FiveLow())
            };

            RunSession run = new RunSession(easyEnemies);
            run.SelectDeck(DefaultCatalog.Equilibrado, new System.Random(7));
            PlayUntilEncounterEnds(run);
            PlayUntilEncounterEnds(run);
            PlayUntilEncounterEnds(run);

            Assert.AreEqual(RunPhase.Victory, run.Phase);
            Assert.AreEqual(3, run.EnemiesDefeated);
        }

        private static IReadOnlyList<Card> FiveLow()
        {
            return new[]
            {
                new Card(Suit.Rock, 1),
                new Card(Suit.Rock, 1),
                new Card(Suit.Rock, 1),
                new Card(Suit.Rock, 1),
                new Card(Suit.Rock, 1)
            };
        }

        private static IReadOnlyList<Card> FiveHigh()
        {
            return new[]
            {
                new Card(Suit.Rock, 5),
                new Card(Suit.Rock, 5),
                new Card(Suit.Rock, 5),
                new Card(Suit.Rock, 5),
                new Card(Suit.Rock, 5)
            };
        }

        private static void PlayUntilEncounterEnds(RunSession run)
        {
            Encounter encounter = run.CurrentEncounter;
            int guard = 0;
            while (encounter != null && !encounter.IsFinished)
            {
                run.PlayFromHand(0);
                if (++guard > 10)
                {
                    Assert.Fail("Loop infinito no encontro.");
                }
            }
        }
    }
}
