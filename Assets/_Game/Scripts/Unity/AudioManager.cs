using RockPaperPistol.Core;
using UnityEngine;

namespace RockPaperPistol.Unity
{
    public sealed class AudioManager : MonoBehaviour
    {
        public GameplayEvent? LastEvent { get; private set; }

        private void OnEnable()
        {
            GameplayEventBus.Raised += OnGameplayEvent;
        }

        private void OnDisable()
        {
            GameplayEventBus.Raised -= OnGameplayEvent;
        }

        public void Play(GameplayEvent gameplayEvent)
        {
            LastEvent = gameplayEvent;
        }

        private void OnGameplayEvent(GameplayEvent gameplayEvent)
        {
            Play(gameplayEvent);
        }
    }
}
