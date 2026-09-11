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
            RunSession run = new RunSession(DefaultCatalog.Enemies);
            run.SelectDeck(DefaultCatalog.BaseDeck, new System.Random(1));

            Card played = FirstBasic(run.Deck.Hand);
            int playerOwnedBefore = CountOwned(run.Deck, played);
            int enemyOwnedBefore = CountOwned(run.EnemyDeck, played);

            run.PlayFromHand(IndexOf(run.Deck.Hand, played));

            Assert.AreEqual(playerOwnedBefore, CountOwned(run.Deck, played));
            Assert.AreEqual(enemyOwnedBefore, CountOwned(run.EnemyDeck, played));
            Assert.IsFalse(ContainsCard(run.Deck.Hand, played));
            Assert.IsTrue(ContainsCard(run.Deck.Discard, played));
        }

        [Test]
        public void RunSession_StartsWithEightBasicsAndOnePistolEach()
        {
            RunSession run = new RunSession(DefaultCatalog.Enemies);
            run.SelectDeck(DefaultCatalog.BaseDeck, new System.Random(1));

            Assert.AreEqual(9, run.Deck.HandCount);
            Assert.AreEqual(9, run.EnemyDeck.HandCount);
            Assert.AreEqual(1, run.Deck.Excluded.Count);
            Assert.AreEqual(1, run.EnemyDeck.Excluded.Count);
            Assert.IsTrue(ContainsCard(run.Deck.Hand, DefaultCatalog.PlayerPistol));
            Assert.IsTrue(ContainsCard(run.EnemyDeck.Hand, Card.CreatePistol(PistolId.Estatua)));
            Assert.AreEqual(Encounter.DefaultMaxTurns, run.MaxTurns);
            CollectionAssert.AreEqual(DefaultCatalog.BaseDeck, run.Deck.Composition);
            CollectionAssert.AreEqual(DefaultCatalog.BaseDeck, run.EnemyDeck.Composition);
        }

        [Test]
        public void RunSession_VictoryResetsDeckForNextEnemy()
        {
            IReadOnlyList<NamedEnemy> easyEnemies = new[]
            {
                Easy("A"),
                Easy("B"),
                Easy("C")
            };

            RunSession run = new RunSession(easyEnemies);
            run.SelectDeck(NineStrong(), new System.Random(1));

            PlayUntilEncounterEnds(run);
            Assert.AreEqual(RunPhase.InEncounter, run.Phase);
            Assert.AreEqual(1, run.EnemyIndex);
            Assert.AreEqual(1, run.EnemiesDefeated);
            Assert.AreEqual(0, run.Deck.DiscardCount);
            Assert.AreEqual(9, run.Deck.HandCount);
            Assert.AreEqual(9, run.EnemyDeck.HandCount);
            Assert.AreEqual(1, run.Deck.Excluded.Count);
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
                Easy("A"),
                Easy("B"),
                Easy("C")
            };

            RunSession run = new RunSession(easyEnemies);
            run.SelectDeck(NineStrong(), new System.Random(7));
            PlayUntilEncounterEnds(run);
            PlayUntilEncounterEnds(run);
            PlayUntilEncounterEnds(run);

            Assert.AreEqual(RunPhase.Victory, run.Phase);
            Assert.AreEqual(3, run.EnemiesDefeated);
        }

        private static NamedEnemy Easy(string name)
        {
            return new NamedEnemy(
                name,
                NineWeak(),
                EnemyBehavior.Defensive,
                Suit.Rock,
                Card.CreatePistol(PistolId.Estatua));
        }

        private static Card FirstBasic(IReadOnlyList<Card> cards)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                if (!cards[i].IsPistol)
                {
                    return cards[i];
                }
            }

            Assert.Fail("Não havia carta básica na mão.");
            return default;
        }

        private static int CountOwned(DeckRuntime deck, Card target)
        {
            return CountCard(deck.Hand, target)
                   + CountCard(deck.Discard, target)
                   + CountCard(deck.Excluded, target)
                   + CountCard(deck.DrawPile, target);
        }

        private static int CountCard(IReadOnlyList<Card> cards, Card target)
        {
            int count = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].Equals(target))
                {
                    count += 1;
                }
            }

            return count;
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
