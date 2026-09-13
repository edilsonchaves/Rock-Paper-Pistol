using System.Collections.Generic;
using RockPaperPistol.Core;
using RockPaperPistol.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RockPaperPistol.Unity
{
    public sealed class AudioManager : MonoBehaviour
    {
        public const string ObjectName = "AudioManager";
        public const string SourcesFolder = "Assets/_Game/Content/AudioSources";

        [SerializeField] private GameAudioSource[] sources;
        [SerializeField] private UnityEngine.AudioSource playbackPrefab;
        [SerializeField] [Range(0f, 1f)] private float musicVolume = 0.28f;
        [SerializeField] [Range(0f, 1f)] private float sfxVolume = 0.85f;

        private readonly Dictionary<GameAudioSource, UnityEngine.AudioSource> _voices =
            new Dictionary<GameAudioSource, UnityEngine.AudioSource>();

        private string _activeScene;

        public static AudioManager Instance { get; private set; }

        public GameplayEvent? LastEvent { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            EnsureInstance();
        }

        public static AudioManager EnsureInstance()
        {
            if (Instance != null)
            {
                return Instance;
            }

#if UNITY_2023_1_OR_NEWER
            AudioManager existing = FindFirstObjectByType<AudioManager>();
#else
            AudioManager existing = FindObjectOfType<AudioManager>();
#endif
            if (existing != null)
            {
                Instance = existing;
                return existing;
            }

            GameObject root = new GameObject(ObjectName);
            DontDestroyOnLoad(root);
            return root.AddComponent<AudioManager>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            if (transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }

            LoadSources();
            _activeScene = SceneManager.GetActiveScene().name;
            PlaySceneEnter(_activeScene);
        }

        private void OnEnable()
        {
            GameplayEventBus.Raised += OnGameplayEvent;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            GameplayEventBus.Raised -= OnGameplayEvent;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void Play(GameplayEvent gameplayEvent)
        {
            LastEvent = gameplayEvent;
            bool pistol = IsSelectedPistol();
            Suit suit = CurrentPlayerSuit();

            for (int i = 0; i < sources.Length; i++)
            {
                GameAudioSource source = sources[i];
                if (source == null)
                {
                    continue;
                }

                if (source.MatchesStop(gameplayEvent))
                {
                    Stop(source);
                }

                if (pistol
                    && gameplayEvent == GameplayEvent.CardSelected
                    && source.PlayTrigger == AudioPlayTrigger.GameplayEvent
                    && source.PlayEvent == GameplayEvent.CardSelected)
                {
                    continue;
                }

                if (source.MatchesPlay(gameplayEvent, pistol, suit))
                {
                    Play(source);
                }
            }
        }

        public void SetPaused(bool paused)
        {
            foreach (UnityEngine.AudioSource voice in _voices.Values)
            {
                if (voice == null)
                {
                    continue;
                }

                if (paused)
                {
                    voice.Pause();
                }
                else
                {
                    voice.UnPause();
                }
            }
        }

        private void OnGameplayEvent(GameplayEvent gameplayEvent)
        {
            Play(gameplayEvent);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            string previous = _activeScene;
            _activeScene = scene.name;
            StopSceneExit(previous);
            PlaySceneEnter(_activeScene);
        }

        private void PlaySceneEnter(string sceneName)
        {
            if (sources == null)
            {
                return;
            }

            for (int i = 0; i < sources.Length; i++)
            {
                GameAudioSource source = sources[i];
                if (source != null && source.MatchesScene(sceneName))
                {
                    Play(source);
                }
            }
        }

        private void StopSceneExit(string sceneName)
        {
            if (sources == null || string.IsNullOrEmpty(sceneName))
            {
                return;
            }

            for (int i = 0; i < sources.Length; i++)
            {
                GameAudioSource source = sources[i];
                if (source != null && source.MatchesSceneExit(sceneName) && !source.MatchesScene(_activeScene))
                {
                    Stop(source);
                }
            }
        }

        private void Play(GameAudioSource source)
        {
            if (source == null || !source.HasFile)
            {
                return;
            }

            UnityEngine.AudioSource voice = VoiceFor(source);
            if (voice == null)
            {
                return;
            }

            if (source.Loop && voice.isPlaying && voice.clip == source.File)
            {
                voice.volume = BusVolume(source) * source.Volume;
                return;
            }

            voice.Stop();
            voice.clip = source.File;
            voice.time = source.ResolvedStart;
            voice.loop = source.Loop;
            voice.volume = BusVolume(source) * source.Volume;
            voice.Play();

            if (!source.Loop && source.StopTrigger == AudioStopTrigger.Duration && source.Duration > 0f)
            {
                voice.SetScheduledEndTime(AudioSettings.dspTime + source.ResolvedDuration);
            }
        }

        private void Stop(GameAudioSource source)
        {
            if (source == null || !_voices.TryGetValue(source, out UnityEngine.AudioSource voice) || voice == null)
            {
                return;
            }

            voice.Stop();
        }

        private UnityEngine.AudioSource VoiceFor(GameAudioSource source)
        {
            if (_voices.TryGetValue(source, out UnityEngine.AudioSource existing) && existing != null)
            {
                return existing;
            }

            UnityEngine.AudioSource voice;
            if (playbackPrefab != null)
            {
                voice = Instantiate(playbackPrefab, transform);
                voice.gameObject.name = source.name;
            }
            else
            {
                GameObject go = new GameObject(source.name);
                go.transform.SetParent(transform, false);
                voice = go.AddComponent<UnityEngine.AudioSource>();
            }

            voice.playOnAwake = false;
            voice.spatialBlend = 0f;
            _voices[source] = voice;
            return voice;
        }

        private float BusVolume(GameAudioSource source)
        {
            return source.Loop || source.PlayTrigger == AudioPlayTrigger.SceneEnter
                ? musicVolume
                : sfxVolume;
        }

        private void LoadSources()
        {
            if (sources != null && sources.Length > 0)
            {
                return;
            }

#if UNITY_EDITOR
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:GameAudioSource", new[] { SourcesFolder });
            sources = new GameAudioSource[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                sources[i] = UnityEditor.AssetDatabase.LoadAssetAtPath<GameAudioSource>(path);
            }
#endif
        }

        private bool IsSelectedPistol()
        {
            GameSessionDriver driver = FindDriver();
            return driver != null
                && driver.SelectedPlayerCard.HasValue
                && driver.SelectedPlayerCard.Value.IsPistol;
        }

        private Suit CurrentPlayerSuit()
        {
            GameSessionDriver driver = FindDriver();
            if (driver != null && driver.LastPlay.HasValue)
            {
                return driver.LastPlay.Value.Round.Resolution.PlayerSuit;
            }

            if (driver != null && driver.SelectedPlayerCard.HasValue)
            {
                return driver.SelectedPlayerCard.Value.Suit;
            }

            return Suit.Rock;
        }

        private static GameSessionDriver FindDriver()
        {
#if UNITY_2023_1_OR_NEWER
            return FindFirstObjectByType<GameSessionDriver>();
#else
            return FindObjectOfType<GameSessionDriver>();
#endif
        }
    }
}
