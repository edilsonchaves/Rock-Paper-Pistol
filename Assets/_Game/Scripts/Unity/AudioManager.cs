using RockPaperPistol.Core;
using UnityEngine;

namespace RockPaperPistol.Unity
{
    public sealed class AudioManager : MonoBehaviour
    {
        public const string ObjectName = "GameAudio";

        [SerializeField] private GameSounds sounds;

        private AudioSource _sfx;
        private AudioSource _music;

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

            _sfx = CreateSource("Sfx", false);
            _music = CreateSource("Music", true);
            LoadCatalog();
            StartMusic();
        }

        private void OnEnable()
        {
            GameplayEventBus.Raised += OnGameplayEvent;
        }

        private void OnDisable()
        {
            GameplayEventBus.Raised -= OnGameplayEvent;
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void Play(GameplayEvent gameplayEvent)
        {
            LastEvent = gameplayEvent;
            if (sounds == null || _sfx == null)
            {
                return;
            }

            AudioClip clip = sounds.ClipFor(gameplayEvent);
            if (clip != null)
            {
                _sfx.PlayOneShot(clip, sounds.SfxVolume);
            }
        }

        public void SetPaused(bool paused)
        {
            if (_music == null)
            {
                return;
            }

            if (paused)
            {
                _music.Pause();
            }
            else
            {
                _music.UnPause();
            }
        }

        private void OnGameplayEvent(GameplayEvent gameplayEvent)
        {
            Play(gameplayEvent);
        }

        private void LoadCatalog()
        {
            if (sounds != null)
            {
                return;
            }

#if UNITY_EDITOR
            sounds = UnityEditor.AssetDatabase.LoadAssetAtPath<GameSounds>(GameSounds.DefaultAssetPath);
#endif
        }

        private void StartMusic()
        {
            if (_music == null || sounds == null || sounds.Music == null)
            {
                return;
            }

            _music.clip = sounds.Music;
            _music.volume = sounds.MusicVolume;
            _music.loop = true;
            if (!_music.isPlaying)
            {
                _music.Play();
            }
        }

        private AudioSource CreateSource(string childName, bool loop)
        {
            Transform child = transform.Find(childName);
            GameObject go = child != null ? child.gameObject : new GameObject(childName);
            if (child == null)
            {
                go.transform.SetParent(transform, false);
            }

            AudioSource source = go.GetComponent<AudioSource>();
            if (source == null)
            {
                source = go.AddComponent<AudioSource>();
            }

            source.playOnAwake = false;
            source.loop = loop;
            source.spatialBlend = 0f;
            return source;
        }
    }
}
