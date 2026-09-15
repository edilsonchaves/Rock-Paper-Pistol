using RockPaperPistol.Utils;

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

        public static Suit WinningSuitAgainst(Suit defender)
        {
            switch (defender)
            {
                case Suit.Rock:
                    return Suit.Paper;
                case Suit.Paper:
                    return Suit.Scissors;
                default:
                    return Suit.Rock;
            }
        }

        public static int MumiaValueForTurn(int turn)
        {
            if (turn <= 2)
            {
                return 1;
            }

            if (turn <= 5)
            {
                return 2;
            }

            return 3;
        }

        public static int AdjustedValue(Card card, Card opponent)
        {
            return Compare(card, opponent).PlayerAdjusted;
        }

        public static RoundResolution Compare(Card player, Card enemy, int stake = 1, int turn = 1)
        {
            if (stake < 1)
            {
                stake = 1;
            }

            if (turn < 1)
            {
                turn = 1;
            }

            Suit playerSuit = ResolveSuit(player, enemy);
            Suit enemySuit = ResolveSuit(enemy, player);
            int playerValue = ResolveValue(player, enemySuit, turn);
            int enemyValue = ResolveValue(enemy, playerSuit, turn);
            int playerAdjusted = ApplySuitBonus(player, playerValue, playerSuit, enemySuit);
            int enemyAdjusted = ApplySuitBonus(enemy, enemyValue, enemySuit, playerSuit);

            RoundOutcome outcome;
            if (playerAdjusted > enemyAdjusted)
            {
                outcome = RoundOutcome.PlayerWin;
            }
            else if (playerAdjusted < enemyAdjusted)
            {
                outcome = RoundOutcome.EnemyWin;
            }
            else if (Beats(playerSuit, enemySuit))
            {
                outcome = RoundOutcome.PlayerWin;
            }
            else if (Beats(enemySuit, playerSuit))
            {
                outcome = RoundOutcome.EnemyWin;
            }
            else
            {
                outcome = RoundOutcome.Draw;
            }

            int stakeAwarded = outcome == RoundOutcome.Draw ? 0 : stake;
            return new RoundResolution(
                outcome,
                playerAdjusted,
                enemyAdjusted,
                stakeAwarded,
                playerSuit,
                enemySuit);
        }

        public static Suit ResolveSuit(Card card, Card opponent)
        {
            if (card.Pistol == PistolId.Pistoleiro)
            {
                return WinningSuitAgainst(opponent.Suit);
            }

            return card.Suit;
        }

        public static int ResolveValue(Card card, Suit opponentEffectiveSuit, int turn)
        {
            if (card.Pistol == PistolId.Mumia)
            {
                return MumiaValueForTurn(turn);
            }

            if (card.Pistol == PistolId.Pirata)
            {
                return HasAdditivePistolBonus(card, opponentEffectiveSuit) ? 3 : 1;
            }

            if (card.Pistol == PistolId.Estatua)
            {
                return opponentEffectiveSuit == Suit.Rock ? 1 : 3;
            }

            return card.Value;
        }

        public static bool HasAdditivePistolBonus(Card card, Suit opponentEffectiveSuit)
        {
            if (card.Pistol == PistolId.Pirata)
            {
                return opponentEffectiveSuit == Suit.Paper || opponentEffectiveSuit == Suit.Scissors;
            }

            if (card.Pistol == PistolId.Estatua)
            {
                return opponentEffectiveSuit == Suit.Paper || opponentEffectiveSuit == Suit.Scissors;
            }

            return false;
        }

        private static int ApplySuitBonus(Card card, int resolvedValue, Suit cardSuit, Suit opponentSuit)
        {
            if (HasAdditivePistolBonus(card, opponentSuit))
            {
                return resolvedValue;
            }

            int bonus = Beats(cardSuit, opponentSuit) ? 1 : 0;
            return resolvedValue + bonus;
        }
    }
}
