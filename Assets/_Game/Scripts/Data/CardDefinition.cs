using RockPaperPistol.Core;
using UnityEngine;

namespace RockPaperPistol.Data
{
    [CreateAssetMenu(fileName = "CardDefinition", menuName = "Scriptable Objects/CardDefinition")]
    public class CardDefinition : ScriptableObject
    {
        public CardKind Kind;
        public Sprite CardImage;
    }
}
