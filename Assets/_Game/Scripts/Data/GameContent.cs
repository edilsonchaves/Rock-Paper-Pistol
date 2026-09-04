using System.Collections.Generic;
using RockPaperPistol.Core;
using UnityEngine;

namespace RockPaperPistol.Data
{
    [CreateAssetMenu(fileName = "GameContent", menuName = "Rock Paper Pistol/Conteúdo do Jogo")]
    public sealed class GameContent : ScriptableObject
    {
        public DeckDefinition[] Decks;
        public EnemyDefinition[] Enemies;

        public IReadOnlyList<NamedDeck> ToDecks()
        {
            if (Decks == null || Decks.Length == 0)
            {
                return DefaultCatalog.Decks;
            }

            NamedDeck[] result = new NamedDeck[Decks.Length];
            for (int i = 0; i < Decks.Length; i++)
            {
                result[i] = Decks[i].ToNamedDeck();
            }

            return result;
        }

        public IReadOnlyList<NamedEnemy> ToEnemies()
        {
            if (Enemies == null || Enemies.Length == 0)
            {
                return DefaultCatalog.Enemies;
            }

            NamedEnemy[] result = new NamedEnemy[Enemies.Length];
            for (int i = 0; i < Enemies.Length; i++)
            {
                result[i] = Enemies[i].ToNamedEnemy();
            }

            return result;
        }
    }
}
