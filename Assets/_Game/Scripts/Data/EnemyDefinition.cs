using System.Collections.Generic;
using RockPaperPistol.Core;
using RockPaperPistol.Utils;
using UnityEngine;

namespace RockPaperPistol.Data
{
    [CreateAssetMenu(fileName = "Enemy", menuName = "Rock Paper Pistol/Inimigo")]
    public sealed class EnemyDefinition : ScriptableObject
    {
        public string DisplayName;
        public Color SpriteColor;
        public EnemyBehavior Behavior = EnemyBehavior.Defensive;
        public Suit PreferredSuit = Suit.Rock;
        public PistolId Pistol = PistolId.None;
        public int PistolAvailableFromTurn = 1;
        public List<CardDefinition> Sequence;
        
        public Sprite ImageRoundWinner;
    }
}
