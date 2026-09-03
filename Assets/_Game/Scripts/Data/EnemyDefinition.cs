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
        public CardData[] Sequence = new CardData[Encounter.RoundsPerEncounter];

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
            return new NamedEnemy(
                string.IsNullOrEmpty(DisplayName) ? name : DisplayName,
                ToSequence(),
                Behavior);
        }
    }
}
