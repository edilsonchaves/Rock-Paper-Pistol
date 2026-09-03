namespace RockPaperPistol.Core
{
    public static class CardComparer
    {
        public static bool Beats(Suit attacker, Suit defender)
        {
            return (attacker == Suit.Paper && defender == Suit.Rock)
                   || (attacker == Suit.Rock && defender == Suit.Scissors)
                   || (attacker == Suit.Scissors && defender == Suit.Paper);
        }

        public static int AdjustedValue(Card card, Card opponent)
        {
            int bonus = Beats(card.Suit, opponent.Suit) ? 1 : 0;
            return card.Value + bonus;
        }

        public static RoundResolution Compare(Card player, Card enemy, int stake = 1)
        {
            if (stake < 1)
            {
                stake = 1;
            }

            int playerAdjusted = AdjustedValue(player, enemy);
            int enemyAdjusted = AdjustedValue(enemy, player);

            RoundOutcome outcome;
            if (playerAdjusted > enemyAdjusted)
            {
                outcome = RoundOutcome.PlayerWin;
            }
            else if (playerAdjusted < enemyAdjusted)
            {
                outcome = RoundOutcome.EnemyWin;
            }
            else if (Beats(player.Suit, enemy.Suit))
            {
                outcome = RoundOutcome.PlayerWin;
            }
            else if (Beats(enemy.Suit, player.Suit))
            {
                outcome = RoundOutcome.EnemyWin;
            }
            else
            {
                outcome = RoundOutcome.Draw;
            }

            int stakeAwarded = outcome == RoundOutcome.Draw ? 0 : stake;
            return new RoundResolution(outcome, playerAdjusted, enemyAdjusted, stakeAwarded);
        }
    }
}
