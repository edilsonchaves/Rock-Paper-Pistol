using System.Collections.Generic;
using RockPaperPistol.Core;
using UnityEngine;

namespace RockPaperPistol.Data
{
    [CreateAssetMenu(fileName = "Enemy", menuName = "Rock Paper Pistol/Inimigo")]
    public sealed class EnemyDefinition : ScriptableObject
    {
        public string DisplayName;
        public EnemyBehavior Behavior = EnemyBehavior.Defensive;
        public Suit PreferredSuit = Suit.Rock;
        public PistolId Pistol = PistolId.None;
        public int PistolAvailableFromTurn = 1;
        public CardData[] Sequence = new CardData[DefaultCatalog.BaseDeckSize];

        public IReadOnlyList<Card> ToSequence()
        {
            Card[] result = new Card[Sequence != null ? Sequence.Length : 0];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Sequence[i].ToCard();
            }

            return result;
        }

        public NamedEnemy ToNamedEnemy()
        {
            Card? pistol = Pistol == PistolId.None ? (Card?)null : Card.CreatePistol(Pistol);
            return new NamedEnemy(
                string.IsNullOrEmpty(DisplayName) ? name : DisplayName,
                ToSequence(),
                Behavior,
                PreferredSuit,
                pistol,
                PistolAvailableFromTurn < 1 ? 1 : PistolAvailableFromTurn);
        }
    }
}
