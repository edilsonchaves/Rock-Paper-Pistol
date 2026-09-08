using System;
using System.Collections.Generic;

namespace RockPaperPistol.Core
{
    public static class EnemyCardPicker
    {
        public const double PreferredSuitChance = 0.35;
        public const int DefaultPistolAvailableFromTurn = 1;

        public static int ChooseHandIndex(
            IReadOnlyList<Card> hand,
            Suit preferredSuit,
            int currentTurn,
            int maxTurns,
            int pistolAvailableFromTurn,
            Random rng)
        {
            if (rng == null)
            {
                throw new ArgumentNullException(nameof(rng));
            }

            return ChooseHandIndex(
                hand,
                preferredSuit,
                currentTurn,
                maxTurns,
                pistolAvailableFromTurn,
                rng.NextDouble(),
                count => rng.Next(count));
        }

        public static int ChooseHandIndex(
            IReadOnlyList<Card> hand,
            Suit preferredSuit,
            int currentTurn,
            int maxTurns,
            int pistolAvailableFromTurn,
            double tendencyRoll,
            Func<int, int> nextIndex)
        {
            if (hand == null)
            {
                throw new ArgumentNullException(nameof(hand));
            }

            if (hand.Count == 0)
            {
                throw new InvalidOperationException("O inimigo não tem cartas para jogar.");
            }

            if (nextIndex == null)
            {
                throw new ArgumentNullException(nameof(nextIndex));
            }

            int forcedPistol = FindForcedPistol(hand, currentTurn, maxTurns);
            if (forcedPistol >= 0)
            {
                return forcedPistol;
            }

            List<int> available = new List<int>();
            List<int> preferred = new List<int>();
            for (int i = 0; i < hand.Count; i++)
            {
                if (!IsPlayable(hand[i], currentTurn, pistolAvailableFromTurn))
                {
                    continue;
                }

                available.Add(i);
                if (hand[i].Suit == preferredSuit)
                {
                    preferred.Add(i);
                }
            }

            if (available.Count == 0)
            {
                return 0;
            }

            if (preferred.Count > 0 && tendencyRoll < PreferredSuitChance)
            {
                return preferred[ClampIndex(nextIndex(preferred.Count), preferred.Count)];
            }

            return available[ClampIndex(nextIndex(available.Count), available.Count)];
        }

        public static bool IsPlayable(Card card, int currentTurn, int pistolAvailableFromTurn)
        {
            if (!card.IsPistol)
            {
                return true;
            }

            return currentTurn >= pistolAvailableFromTurn;
        }

        private static int FindForcedPistol(IReadOnlyList<Card> hand, int currentTurn, int maxTurns)
        {
            if (currentTurn < maxTurns)
            {
                return -1;
            }

            for (int i = 0; i < hand.Count; i++)
            {
                if (hand[i].IsPistol)
                {
                    return i;
                }
            }

            return -1;
        }

        private static int ClampIndex(int index, int count)
        {
            if (index < 0)
            {
                return 0;
            }

            if (index >= count)
            {
                return count - 1;
            }

            return index;
        }
    }
}
