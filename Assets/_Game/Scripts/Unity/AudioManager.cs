using System.Collections.Generic;
using RockPaperPistol.Core;
using RockPaperPistol.Unity.Battle;
using UnityEngine;

namespace RockPaperPistol.Unity
{
    public sealed class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioClip cardSelected;
        [SerializeField] private AudioClip cardsRevealed;
        [SerializeField] private AudioClip suitChecked;
        [SerializeField] private AudioClip valueChecked;
        [SerializeField] private AudioClip playerWin;
        [SerializeField] private AudioClip playerLose;
        [SerializeField] private AudioClip draw;
        [SerializeField] private AudioClip turnStart;
        [SerializeField] private AudioClip gameWin;
        [SerializeField] private AudioClip gameLose;

        private AudioSource _source;
        private readonly Dictionary<GameplayEvent, AudioClip> _clips = new Dictionary<GameplayEvent, AudioClip>();

        public GameplayEvent? LastEvent { get; private set; }

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
            if (_source == null)
            {
                _source = gameObject.AddComponent<AudioSource>();
            }

            _source.playOnAwake = false;
            Bind(GameplayEvent.CardSelected, cardSelected, "CardSelected", 440f, 0.12f);
            Bind(GameplayEvent.CardsRevealed, cardsRevealed, "CardsRevealed", 330f, 0.16f);
            Bind(GameplayEvent.SuitChecked, suitChecked, "SuitChecked", 520f, 0.14f);
            Bind(GameplayEvent.ValueChecked, valueChecked, "ValueChecked", 620f, 0.14f);
            Bind(GameplayEvent.PlayerWin, playerWin, "PlayerWin", 784f, 0.22f);
            Bind(GameplayEvent.PlayerLose, playerLose, "PlayerLose", 196f, 0.22f);
            Bind(GameplayEvent.Draw, draw, "Draw", 392f, 0.18f);
            Bind(GameplayEvent.TurnStart, turnStart, "TurnStart", 262f, 0.12f);
            Bind(GameplayEvent.GameWin, gameWin, "GameWin", 880f, 0.28f);
            Bind(GameplayEvent.GameLose, gameLose, "GameLose", 147f, 0.28f);
        }

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
            if (_clips.TryGetValue(gameplayEvent, out AudioClip clip) && clip != null && _source != null)
            {
                _source.PlayOneShot(clip);
            }
        }

        private void OnGameplayEvent(GameplayEvent gameplayEvent)
        {
            Play(gameplayEvent);
        }

        private void Bind(GameplayEvent gameplayEvent, AudioClip assigned, string name, float frequency, float seconds)
        {
            _clips[gameplayEvent] = assigned != null
                ? assigned
                : PlaceholderArt.Tone(name, frequency, seconds);
        }
    }
}
