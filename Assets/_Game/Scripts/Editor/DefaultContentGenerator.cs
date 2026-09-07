using System.IO;
using RockPaperPistol.Core;
using RockPaperPistol.Data;
using UnityEditor;
using UnityEngine;

namespace RockPaperPistol.EditorTools
{
    public static class DefaultContentGenerator
    {
        private const string Root = "Assets/_Game/Content";

        [MenuItem("Rock Paper Pistol/Gerar conteúdo padrão")]
        public static void Generate()
        {
            if (!AssetDatabase.IsValidFolder(Root))
            {
                AssetDatabase.CreateFolder("Assets/_Game", "Content");
            }

            EnsureFolder(Root + "/Decks");
            EnsureFolder(Root + "/Enemies");

            DeckDefinition baseDeck = WriteDeck(DefaultCatalog.BaseDeckName, DefaultCatalog.BaseDeck);

            EnemyDefinition estatua = WriteEnemy(
                "EstatuaDePedra",
                "Estátua de Pedra",
                DefaultCatalog.BaseDeck,
                EnemyBehavior.Defensive);
            EnemyDefinition mumia = WriteEnemy(
                "Mumia",
                "Múmia",
                DefaultCatalog.BaseDeck,
                EnemyBehavior.Defensive);
            EnemyDefinition pirata = WriteEnemy(
                "Pirata",
                "Pirata",
                DefaultCatalog.BaseDeck,
                EnemyBehavior.Aggressive);

            GameContent content = LoadOrCreate<GameContent>(Root + "/GameContent.asset");
            content.Decks = new[] { baseDeck };
            content.Enemies = new[] { estatua, mumia, pirata };
            EditorUtility.SetDirty(content);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = content;
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

        private static DeckDefinition WriteDeck(string displayName, System.Collections.Generic.IReadOnlyList<Card> cards)
        {
            string path = $"{Root}/Decks/{Sanitize(displayName)}.asset";
            DeckDefinition deck = LoadOrCreate<DeckDefinition>(path);
            deck.DisplayName = displayName;
            deck.Cards = ToData(cards);
            EditorUtility.SetDirty(deck);
            return deck;
        }

        private static EnemyDefinition WriteEnemy(
            string fileName,
            string displayName,
            System.Collections.Generic.IReadOnlyList<Card> sequence,
            EnemyBehavior behavior)
        {
            string path = $"{Root}/Enemies/{fileName}.asset";
            EnemyDefinition enemy = LoadOrCreate<EnemyDefinition>(path);
            enemy.DisplayName = displayName;
            enemy.Behavior = behavior;
            enemy.Sequence = ToData(sequence);
            EditorUtility.SetDirty(enemy);
            return enemy;
        }

        private static CardData[] ToData(System.Collections.Generic.IReadOnlyList<Card> cards)
        {
            CardData[] data = new CardData[cards.Count];
            for (int i = 0; i < cards.Count; i++)
            {
                data[i] = new CardData(cards[i].Suit, cards[i].Value);
            }

            return data;
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

        private static string Sanitize(string name)
        {
            return name.Replace("á", "a").Replace("Á", "A").Replace(" ", "");
        }
    }
}
