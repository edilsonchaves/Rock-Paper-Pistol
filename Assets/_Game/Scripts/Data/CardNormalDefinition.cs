using RockPaperPistol.Core;

using UnityEngine;

namespace RockPaperPistol.Data
{
    [CreateAssetMenu(fileName = "CardNormalDefinition", menuName = "Rock Paper Pistol/CardNormalDefinition")]
    public class CardNormalDefinition : CardDefinition
    {
        public Suit Suit;
        [Range(Card.MinValue, Card.MaxValue)]

        public int Value;
    }
}
