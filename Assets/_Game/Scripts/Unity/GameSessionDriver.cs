using System.Collections.Generic;
using RockPaperPistol.Core;
using RockPaperPistol.Data;
using UnityEngine;

namespace RockPaperPistol.Unity
{
    public sealed class GameSessionDriver : MonoBehaviour
    {
        [SerializeField] private GameContent content;

        private RunSession _session;
        private IReadOnlyList<NamedDeck> _decks;
        private PlayResult? _lastPlay;

        public RunSession Session
        {
            get
            {
                EnsureSession();
                return _session;
            }
        }

        public IReadOnlyList<NamedDeck> Decks
        {
            get
            {
                EnsureSession();
                return _decks;
            }
        }

        public PlayResult? LastPlay => _lastPlay;

        public GameContent Content => content;

        private void Awake()
        {
            EnsureSession();
        }

        public void EnsureSession()
        {
            if (_session != null)
            {
                return;
            }

            LoadContentIfNeeded();
            _decks = content != null ? content.ToDecks() : DefaultCatalog.Decks;
            IReadOnlyList<NamedEnemy> enemies = content != null ? content.ToEnemies() : DefaultCatalog.Enemies;
            _session = new RunSession(enemies);
        }

        public void SelectDeck(int index)
        {
            EnsureSession();
            if (index < 0 || index >= _decks.Count)
            {
                throw new System.ArgumentOutOfRangeException(nameof(index));
            }

            _lastPlay = null;
            _session.SelectDeck(_decks[index].Cards);
        }

        public PlayResult PlayFromHand(int handIndex)
        {
            EnsureSession();
            PlayResult result = _session.PlayFromHand(handIndex);
            _lastPlay = result;
            return result;
        }

        public void Restart()
        {
            _lastPlay = null;
            LoadContentIfNeeded();
            IReadOnlyList<NamedEnemy> enemies = content != null ? content.ToEnemies() : DefaultCatalog.Enemies;
            _decks = content != null ? content.ToDecks() : DefaultCatalog.Decks;
            _session = new RunSession(enemies);
        }

        private void LoadContentIfNeeded()
        {
#if UNITY_EDITOR
            if (content == null)
            {
                content = UnityEditor.AssetDatabase.LoadAssetAtPath<GameContent>(
                    "Assets/Content/GameContent.asset");
            }
#endif
        }
    }
}
