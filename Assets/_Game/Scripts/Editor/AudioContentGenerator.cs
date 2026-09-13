using System.IO;
using RockPaperPistol.Core;
using RockPaperPistol.Data;
using RockPaperPistol.Unity;
using UnityEditor;
using UnityEngine;

namespace RockPaperPistol.EditorTools
{
    public static class AudioContentGenerator
    {
        private const string ClipsRoot = "Assets/_Game/Content/Clips";
        private const string SourcesRoot = "Assets/_Game/Content/AudioSources";
        private const string ManagerPrefab = "Assets/_Game/Prefabs/AudioManager.prefab";
        private const string PlaybackPrefab = "Assets/_Game/Prefabs/AudioSource.prefab";

        [MenuItem("Rock Paper Pistol/Gerar áudio padrão")]
        public static void Generate()
        {
            EnsureFolder(ClipsRoot);
            EnsureFolder(SourcesRoot);

            Clip carnival = WriteClip("Carnival", "phantasticbeats-carnival-109979");
            Clip cardFlip = WriteClip("CardFlip", "freesound_community-shuffleandcardflip1-40942");
            Clip deckCards = WriteClip("DeckCards", "freesound_community-baralho-101467");
            Clip chips = WriteClip("Chips", "oxidvideos-placing-poker-chips-522515");
            Clip gunshot = WriteClip("Gunshot", "universfield-gunshot-352466");
            Clip rockThud = WriteClip("RockThud", "dragon-studio-heavy-boulder-thud-515257");
            Clip paperSlide = WriteClip("PaperSlide", "freesound_community-papel-empujado-100951");
            Clip scissorsCut = WriteClip("ScissorsCut", "spinopel-cut-using-a-scissor-429792");
            Clip goodResult = WriteClip("GoodResult", "freesound_community-goodresult-82807");
            Clip impact = WriteClip("Impact", "primalhousemusic-production-elements-impactor-e-188986");
            Clip levelUpAlt = WriteClip("LevelUpAlt", "u_c0n7gy2b9p-level-up-232906");
            Clip youWin = WriteClip("YouWin", "mrstokes302-you-win-sfx-mrstokes302-442128");
            WriteClip("Scissors", "freesound_community-scissors-69248");
            WriteClip("LevelUp", "tithuh-level-up-0-523643");
            WriteClip("Applause", "driken5482-applause-cheer-236786");
            WriteClip("YeahBoy", "universfield-yeah-boy-114748");
            WriteClip("Fanfare", "freesound_community-taratata-6264");

            GameAudioSource[] catalog =
            {
                WriteMusic("Music_MainMenu", carnival, "MainMenu", 1f),
                WriteMusic("Music_WeaponScene", carnival, "WeaponScene", 0.85f),
                WriteMusic("Music_Battle", carnival, "SampleScene", 0.7f),
                WriteSfx("Sfx_TurnStart", deckCards, GameplayEvent.TurnStart, 0.7f),
                WriteSfx("Sfx_CardSelected", cardFlip, GameplayEvent.CardSelected, 0.55f),
                WritePistol("Sfx_Pistol", gunshot),
                WriteSfx("Sfx_CardsRevealed", cardFlip, GameplayEvent.CardsRevealed, 0.55f),
                WriteSuit("Sfx_Rock", rockThud, Suit.Rock, 0.6f),
                WriteSuit("Sfx_Paper", paperSlide, Suit.Paper, 0f),
                WriteSuit("Sfx_Scissors", scissorsCut, Suit.Scissors, 0.5f),
                WriteSfx("Sfx_ValueChecked", chips, GameplayEvent.ValueChecked, 0f, 0.8f),
                WriteSfx("Sfx_PlayerWin", goodResult, GameplayEvent.PlayerWin, 0f),
                WriteSfx("Sfx_PlayerLose", impact, GameplayEvent.PlayerLose, 0.75f),
                WriteSfx("Sfx_Draw", levelUpAlt, GameplayEvent.Draw, 0.6f),
                WriteSfx("Sfx_CardDiscarded", chips, GameplayEvent.CardDiscarded, 0f),
                WriteSfx("Sfx_GameWin", youWin, GameplayEvent.GameWin, 0f),
                WriteSfx("Sfx_GameLose", impact, GameplayEvent.GameLose, 0.9f)
            };

            AssignManager(catalog);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(ManagerPrefab);
        }

        private static Clip WriteClip(string name, string fileName)
        {
            string path = $"{ClipsRoot}/{name}.asset";
            Clip clip = LoadOrCreate<Clip>(path);
            SerializedObject so = new SerializedObject(clip);
            so.FindProperty("file").objectReferenceValue = FindAudio(fileName);
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(clip);
            return clip;
        }

        private static GameAudioSource WriteMusic(string name, Clip clip, string sceneName, float volume)
        {
            return WriteSource(name, clip, AudioPlayTrigger.SceneEnter, sceneName, default, default,
                AudioStopTrigger.SceneExit, default, 0f, 0f, true, volume);
        }

        private static GameAudioSource WriteSfx(
            string name,
            Clip clip,
            GameplayEvent playEvent,
            float duration,
            float volume = 1f)
        {
            return WriteSource(name, clip, AudioPlayTrigger.GameplayEvent, string.Empty, playEvent, default,
                AudioStopTrigger.Duration, default, 0f, duration, false, volume);
        }

        private static GameAudioSource WritePistol(string name, Clip clip)
        {
            return WriteSource(name, clip, AudioPlayTrigger.PistolCard, string.Empty, GameplayEvent.CardSelected, default,
                AudioStopTrigger.Duration, default, 0f, 0f, false, 1f);
        }

        private static GameAudioSource WriteSuit(string name, Clip clip, Suit suit, float duration)
        {
            return WriteSource(name, clip, AudioPlayTrigger.SuitCheck, string.Empty, GameplayEvent.SuitChecked, suit,
                AudioStopTrigger.Duration, default, 0f, duration, false, 1f);
        }

        private static GameAudioSource WriteSource(
            string name,
            Clip clip,
            AudioPlayTrigger playTrigger,
            string sceneName,
            GameplayEvent playEvent,
            Suit suit,
            AudioStopTrigger stopTrigger,
            GameplayEvent stopEvent,
            float startTime,
            float duration,
            bool loop,
            float volume)
        {
            GameAudioSource source = LoadOrCreate<GameAudioSource>($"{SourcesRoot}/{name}.asset");
            SerializedObject so = new SerializedObject(source);
            so.FindProperty("clip").objectReferenceValue = clip;
            so.FindProperty("playTrigger").enumValueIndex = (int)playTrigger;
            so.FindProperty("sceneName").stringValue = sceneName;
            so.FindProperty("playEvent").enumValueIndex = (int)playEvent;
            so.FindProperty("suit").enumValueIndex = (int)suit;
            so.FindProperty("stopTrigger").enumValueIndex = (int)stopTrigger;
            so.FindProperty("stopEvent").enumValueIndex = (int)stopEvent;
            so.FindProperty("startTime").floatValue = startTime;
            so.FindProperty("duration").floatValue = duration;
            so.FindProperty("loop").boolValue = loop;
            so.FindProperty("volume").floatValue = volume;
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(source);
            return source;
        }

        private static void AssignManager(GameAudioSource[] catalog)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(ManagerPrefab);
            if (prefab == null)
            {
                return;
            }

            AudioManager manager = prefab.GetComponent<AudioManager>();
            if (manager == null)
            {
                return;
            }

            SerializedObject so = new SerializedObject(manager);
            SerializedProperty list = so.FindProperty("sources");
            list.arraySize = catalog.Length;
            for (int i = 0; i < catalog.Length; i++)
            {
                list.GetArrayElementAtIndex(i).objectReferenceValue = catalog[i];
            }

            so.FindProperty("playbackPrefab").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<UnityEngine.AudioSource>(PlaybackPrefab);
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(prefab);
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            string parent = Path.GetDirectoryName(path)?.Replace("\\", "/");
            string name = Path.GetFileName(path);
            if (!string.IsNullOrEmpty(parent) && !string.IsNullOrEmpty(name))
            {
                AssetDatabase.CreateFolder(parent, name);
            }
        }

        private static T LoadOrCreate<T>(string path) where T : ScriptableObject
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null)
            {
                return asset;
            }

            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static AudioClip FindAudio(string fileName)
        {
            string[] guids = AssetDatabase.FindAssets(fileName + " t:AudioClip", new[] { "Assets/_Game/Sounds" });
            if (guids.Length == 0)
            {
                Debug.LogWarning($"Clip não encontrado: {fileName}");
                return null;
            }

            return AssetDatabase.LoadAssetAtPath<AudioClip>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }
    }
}
