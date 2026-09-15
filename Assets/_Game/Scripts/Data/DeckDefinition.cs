using System.Collections.Generic;
using RockPaperPistol.Core;
using UnityEngine;

namespace RockPaperPistol.Data
{
    [CreateAssetMenu(fileName = "Deck", menuName = "Rock Paper Pistol/Baralho")]
    public sealed class DeckDefinition : ScriptableObject
    {
        public string DisplayName;
        public List<CardDefinition> Sequence;
        public Sprite ImageRoundWinner;

    }
}
