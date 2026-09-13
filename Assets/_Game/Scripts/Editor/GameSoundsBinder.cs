using UnityEditor;
using UnityEngine;
using RockPaperPistol.Unity;

namespace RockPaperPistol.EditorTools
{
    public static class GameSoundsBinder
    {
        [MenuItem("Rock Paper Pistol/Vincular sons padrão")]
        public static void Bind()
        {
            GameSounds asset = LoadOrCreate();
            SerializedObject so = new SerializedObject(asset);

            Assign(so, "carnival", "phantasticbeats-carnival-109979");
            Assign(so, "cardFlip", "freesound_community-shuffleandcardflip1-40942");
            Assign(so, "deckCards", "freesound_community-baralho-101467");
            Assign(so, "chips", "oxidvideos-placing-poker-chips-522515");
            Assign(so, "gunshot", "universfield-gunshot-352466");
            Assign(so, "rockThud", "dragon-studio-heavy-boulder-thud-515257");
            Assign(so, "paperSlide", "freesound_community-papel-empujado-100951");
            Assign(so, "scissors", "freesound_community-scissors-69248");
            Assign(so, "scissorsCut", "spinopel-cut-using-a-scissor-429792");
            Assign(so, "goodResult", "freesound_community-goodresult-82807");
            Assign(so, "impact", "primalhousemusic-production-elements-impactor-e-188986");
            Assign(so, "levelUp", "tithuh-level-up-0-523643");
            Assign(so, "levelUpAlt", "u_c0n7gy2b9p-level-up-232906");
            Assign(so, "youWin", "mrstokes302-you-win-sfx-mrstokes302-442128");
            Assign(so, "applause", "driken5482-applause-cheer-236786");
            Assign(so, "yeahBoy", "universfield-yeah-boy-114748");
            Assign(so, "fanfare", "freesound_community-taratata-6264");

            Assign(so, "musicLoop", "phantasticbeats-carnival-109979");
            Assign(so, "onTurnStart", "freesound_community-baralho-101467");
            Assign(so, "onCardSelected", "freesound_community-shuffleandcardflip1-40942");
            Assign(so, "onCardsRevealed", "freesound_community-shuffleandcardflip1-40942");
            Assign(so, "onPlayerWin", "freesound_community-goodresult-82807");
            Assign(so, "onPlayerLose", "primalhousemusic-production-elements-impactor-e-188986");
            Assign(so, "onDraw", "u_c0n7gy2b9p-level-up-232906");
            Assign(so, "onCardDiscarded", "oxidvideos-placing-poker-chips-522515");
            Assign(so, "onGameWin", "mrstokes302-you-win-sfx-mrstokes302-442128");
            Assign(so, "onGameLose", "primalhousemusic-production-elements-impactor-e-188986");

            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }

        private static GameSounds LoadOrCreate()
        {
            GameSounds asset = AssetDatabase.LoadAssetAtPath<GameSounds>(GameSounds.DefaultAssetPath);
            if (asset != null)
            {
                return asset;
            }

            asset = ScriptableObject.CreateInstance<GameSounds>();
            AssetDatabase.CreateAsset(asset, GameSounds.DefaultAssetPath);
            return asset;
        }

        private static void Assign(SerializedObject so, string property, string fileName)
        {
            SerializedProperty field = so.FindProperty(property);
            if (field == null)
            {
                Debug.LogWarning($"GameSounds não tem o campo '{property}'.");
                return;
            }

            AudioClip clip = FindClip(fileName);
            if (clip == null)
            {
                Debug.LogWarning($"Clip não encontrado: {fileName}");
                return;
            }

            field.objectReferenceValue = clip;
        }

        private static AudioClip FindClip(string fileName)
        {
            string[] guids = AssetDatabase.FindAssets(fileName + " t:AudioClip");
            if (guids.Length == 0)
            {
                return null;
            }

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<AudioClip>(path);
        }
    }
}
