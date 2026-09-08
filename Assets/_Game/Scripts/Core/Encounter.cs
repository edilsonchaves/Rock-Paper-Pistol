using System;

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
        public const int DefaultMaxTurns = 7;
        public const int RoundsPerEncounter = DefaultMaxTurns;

        private readonly int _maxTurns;
        private int _stake = 1;

        public Encounter(int maxTurns = DefaultMaxTurns)
        {
            if (maxTurns < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxTurns), maxTurns, "MAX_TURNS deve ser pelo menos 1.");
            }

            _maxTurns = maxTurns;
        }

        public int MaxTurns => _maxTurns;
        public int PlayerScore { get; private set; }
        public int EnemyScore { get; private set; }
        public int CurrentStake => _stake;
        public int RoundsPlayed { get; private set; }
        public int RoundsRemaining => _maxTurns - RoundsPlayed;
        public bool IsFinished => RoundsPlayed >= _maxTurns;

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

        public EncounterRoundResult PlayRound(Card player, Card enemy)
        {
            if (IsFinished)
            {
                throw new InvalidOperationException("O encontro já terminou.");
            }

            RoundResolution resolution = CardComparer.Compare(player, enemy, _stake, RoundsPlayed + 1);

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
