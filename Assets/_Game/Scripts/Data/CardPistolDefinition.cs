using UnityEngine;
using RockPaperPistol.Core;
using RockPaperPistol.Utils;

namespace RockPaperPistol.Data
{
    [CreateAssetMenu(fileName = "CardPistol", menuName = "Rock Paper Pistol/CardPistol")]
    public class CardPistolDefinition : CardDefinition
    {
        public PistolId Type;
        public Suit Suit;
        [Range(Card.MinValue, Card.MaxValue)]
        public int Value;
    }
}
