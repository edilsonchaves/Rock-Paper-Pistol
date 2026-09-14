using UnityEngine;

namespace RockPaperPistol.Data
{
    [CreateAssetMenu(fileName = "Clip", menuName = "Rock Paper Pistol/Clip")]
    public sealed class Clip : ScriptableObject
    {
        [Tooltip("Arquivo de áudio em Assets/_Game/Sounds.")]
        [SerializeField] private AudioClip file;

        public AudioClip File => file;
    }
}
