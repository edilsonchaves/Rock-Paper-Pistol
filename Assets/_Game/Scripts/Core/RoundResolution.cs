namespace RockPaperPistol.Core
{
    public enum RoundOutcome
    {
        PlayerWin,
        EnemyWin,
        Draw
    }

    public readonly struct RoundResolution
    {
        public RoundResolution(
            RoundOutcome outcome,
            int playerAdjusted,
            int enemyAdjusted,
            int stakeAwarded)
        {
            Outcome = outcome;
            PlayerAdjusted = playerAdjusted;
            EnemyAdjusted = enemyAdjusted;
            StakeAwarded = stakeAwarded;
        }

        public RoundOutcome Outcome { get; }
        public int PlayerAdjusted { get; }
        public int EnemyAdjusted { get; }
        public int StakeAwarded { get; }

        public bool IsDraw => Outcome == RoundOutcome.Draw;
    }
}
