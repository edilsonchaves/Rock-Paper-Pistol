using System;
using System.Collections.Generic;

namespace RockPaperPistol.Core
{
    public enum EncounterStatus
    {
        InProgress,
        PlayerWon,
        PlayerLost
    }

    public readonly struct EncounterRoundResult
    {
        public EncounterRoundResult(
            Card playerCard,
            Card enemyCard,
            RoundResolution resolution,
            int playerScore,
            int enemyScore,
            int nextStake,
            EncounterStatus status)
        {
            PlayerCard = playerCard;
            EnemyCard = enemyCard;
            Resolution = resolution;
            PlayerScore = playerScore;
            EnemyScore = enemyScore;
            NextStake = nextStake;
            Status = status;
        }

        public Card PlayerCard { get; }
        public Card EnemyCard { get; }
        public RoundResolution Resolution { get; }
        public int PlayerScore { get; }
        public int EnemyScore { get; }
        public int NextStake { get; }
        public EncounterStatus Status { get; }
    }

    public sealed class Encounter
    {
        public const int RoundsPerEncounter = 5;

        private readonly Card[] _enemySequence;
        private int _stake = 1;

        public Encounter(IReadOnlyList<Card> enemySequence)
        {
            if (enemySequence == null)
            {
                throw new ArgumentNullException(nameof(enemySequence));
            }

            if (enemySequence.Count != RoundsPerEncounter)
            {
                throw new ArgumentException(
                    $"O inimigo deve ter exatamente {RoundsPerEncounter} cartas.",
                    nameof(enemySequence));
            }

            _enemySequence = new Card[RoundsPerEncounter];
            for (int i = 0; i < RoundsPerEncounter; i++)
            {
                _enemySequence[i] = enemySequence[i];
            }
        }

        public int PlayerScore { get; private set; }
        public int EnemyScore { get; private set; }
        public int CurrentStake => _stake;
        public int RoundsPlayed { get; private set; }
        public int RoundsRemaining => RoundsPerEncounter - RoundsPlayed;
        public bool IsFinished => RoundsPlayed >= RoundsPerEncounter;

        public EncounterStatus Status
        {
            get
            {
                if (!IsFinished)
                {
                    return EncounterStatus.InProgress;
                }

                return PlayerScore > EnemyScore
                    ? EncounterStatus.PlayerWon
                    : EncounterStatus.PlayerLost;
            }
        }

        public EncounterRoundResult PlayRound(Card player)
        {
            if (IsFinished)
            {
                throw new InvalidOperationException("O encontro já terminou.");
            }

            Card enemy = _enemySequence[RoundsPlayed];
            RoundResolution resolution = CardComparer.Compare(player, enemy, _stake);

            if (resolution.Outcome == RoundOutcome.Draw)
            {
                _stake += 1;
            }
            else
            {
                if (resolution.Outcome == RoundOutcome.PlayerWin)
                {
                    PlayerScore += resolution.StakeAwarded;
                }
                else
                {
                    EnemyScore += resolution.StakeAwarded;
                }

                _stake = 1;
            }

            RoundsPlayed += 1;
            return new EncounterRoundResult(
                player,
                enemy,
                resolution,
                PlayerScore,
                EnemyScore,
                _stake,
                Status);
        }
    }
}
