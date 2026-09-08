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

            EnemyDefinition estatua = WriteEnemy(DefaultCatalog.Enemies[0], "EstatuaDePedra");
            EnemyDefinition mumia = WriteEnemy(DefaultCatalog.Enemies[1], "Mumia");
            EnemyDefinition pirata = WriteEnemy(DefaultCatalog.Enemies[2], "Pirata");

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

        private static EnemyDefinition WriteEnemy(NamedEnemy named, string fileName)
        {
            string path = $"{Root}/Enemies/{fileName}.asset";
            EnemyDefinition enemy = LoadOrCreate<EnemyDefinition>(path);
            enemy.DisplayName = named.Name;
            enemy.Behavior = named.Behavior;
            enemy.PreferredSuit = named.PreferredSuit;
            enemy.Pistol = named.Pistol.HasValue ? named.Pistol.Value.Pistol : PistolId.None;
            enemy.PistolAvailableFromTurn = named.PistolAvailableFromTurn;
            enemy.Sequence = ToData(named.Sequence);
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
