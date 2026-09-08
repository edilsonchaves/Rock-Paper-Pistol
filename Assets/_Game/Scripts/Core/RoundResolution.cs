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
            int stakeAwarded,
            Suit playerSuit = Suit.Rock,
            Suit enemySuit = Suit.Rock)
        {
            Outcome = outcome;
            PlayerAdjusted = playerAdjusted;
            EnemyAdjusted = enemyAdjusted;
            StakeAwarded = stakeAwarded;
            PlayerSuit = playerSuit;
            EnemySuit = enemySuit;
        }

        public RoundOutcome Outcome { get; }
        public int PlayerAdjusted { get; }
        public int EnemyAdjusted { get; }
        public int StakeAwarded { get; }
        public Suit PlayerSuit { get; }
        public Suit EnemySuit { get; }

        public bool IsDraw => Outcome == RoundOutcome.Draw;
    }
}
