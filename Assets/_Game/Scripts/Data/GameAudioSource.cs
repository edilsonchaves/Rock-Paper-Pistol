using RockPaperPistol.Core;
using UnityEngine;

namespace RockPaperPistol.Data
{
    [CreateAssetMenu(fileName = "AudioSource", menuName = "Rock Paper Pistol/Audio Source")]
    public sealed class GameAudioSource : ScriptableObject
    {
        [Header("Arquivo")]
        [Tooltip("Clip com o arquivo de áudio.")]
        [SerializeField] private Clip clip;

        [Header("Gatilho para começar")]
        [SerializeField] private AudioPlayTrigger playTrigger = AudioPlayTrigger.GameplayEvent;

        [Tooltip("Cena em que toca quando o gatilho é SceneEnter. Ex.: MainMenu, WeaponScene, SampleScene.")]
        [SerializeField] private string sceneName;

        [Tooltip("Evento da mesa quando o gatilho é GameplayEvent.")]
        [SerializeField] private GameplayEvent playEvent;

        [Tooltip("Naipe quando o gatilho é SuitCheck.")]
        [SerializeField] private Suit suit;

        [Header("Quando para")]
        [SerializeField] private AudioStopTrigger stopTrigger = AudioStopTrigger.Duration;

        [Tooltip("Evento que interrompe o som quando o stop é GameplayEvent.")]
        [SerializeField] private GameplayEvent stopEvent;

        [Header("Janela do som")]
        [Min(0f)]
        [Tooltip("Onset: segundo em que o arquivo começa a tocar.")]
        [SerializeField] private float startTime;

        [Min(0f)]
        [Tooltip("Duração depois do onset. 0 = até o fim do arquivo, ou loop se Loop estiver ligado.")]
        [SerializeField] private float duration;

        [SerializeField] private bool loop;

        [Range(0f, 1f)]
        [SerializeField] private float volume = 1f;

        public Clip Clip => clip;
        public AudioClip File => clip != null ? clip.File : null;
        public AudioPlayTrigger PlayTrigger => playTrigger;
        public string SceneName => sceneName;
        public GameplayEvent PlayEvent => playEvent;
        public Suit Suit => suit;
        public AudioStopTrigger StopTrigger => stopTrigger;
        public GameplayEvent StopEvent => stopEvent;
        public float StartTime => startTime;
        public float Duration => duration;
        public bool Loop => loop;
        public float Volume => volume > 0f ? volume : 1f;

        public bool HasFile => File != null;

        public float ResolvedStart
        {
            get
            {
                if (File == null)
                {
                    return 0f;
                }

                return Mathf.Clamp(startTime, 0f, Mathf.Max(0f, File.length - 0.02f));
            }
        }

        public float ResolvedDuration
        {
            get
            {
                if (File == null)
                {
                    return 0f;
                }

                float remaining = Mathf.Max(0f, File.length - ResolvedStart);
                return duration > 0f ? Mathf.Min(duration, remaining) : remaining;
            }
        }

        public bool MatchesScene(string activeScene)
        {
            return playTrigger == AudioPlayTrigger.SceneEnter
                && string.Equals(sceneName, activeScene, System.StringComparison.Ordinal);
        }

        public bool MatchesPlay(GameplayEvent gameplayEvent, bool pistolSelected, Suit currentSuit)
        {
            switch (playTrigger)
            {
                case AudioPlayTrigger.GameplayEvent:
                    return gameplayEvent == playEvent;
                case AudioPlayTrigger.PistolCard:
                    return gameplayEvent == GameplayEvent.CardSelected && pistolSelected;
                case AudioPlayTrigger.SuitCheck:
                    return gameplayEvent == GameplayEvent.SuitChecked && currentSuit == suit;
                default:
                    return false;
            }
        }

        public bool MatchesStop(GameplayEvent gameplayEvent)
        {
            return stopTrigger == AudioStopTrigger.GameplayEvent && gameplayEvent == stopEvent;
        }

        public bool MatchesSceneExit(string leavingScene)
        {
            return stopTrigger == AudioStopTrigger.SceneExit
                && string.Equals(sceneName, leavingScene, System.StringComparison.Ordinal);
        }
    }
}
