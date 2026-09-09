using System.Collections;
using System.Collections.Generic;
using RockPaperPistol.Core;
using RockPaperPistol.Data;
using UnityEngine;

namespace RockPaperPistol.Unity
{
    public enum ResolutionStep
    {
        None,
        Selected,
        Revealed,
        SuitChecked,
        ValueChecked,
        ResultShown
    }

    public sealed class GameSessionDriver : MonoBehaviour
    {
        public const float DefaultSelectedDelay = 0.6f;
        public const float DefaultRevealedDelay = 0.6f;
        public const float DefaultRevealDelay = DefaultRevealedDelay;
        public const float DefaultSuitCheckDelay = 0.6f;
        public const float DefaultValueCheckDelay = 0.6f;
        public const float DefaultResultDelay = 0.8f;
        public const float DefaultNextTurnDelay = 0.5f;
        public const string DefaultTimingAssetPath = "Assets/_Game/Content/BattleTiming.asset";

        [SerializeField] private GameContent content;
        [SerializeField] private BattleTiming timing;

        private RunSession _session;
        private IReadOnlyList<NamedDeck> _decks;
        private PlayResult? _lastPlay;
        private Coroutine _resolution;

        public BattleTiming Timing
        {
            get
            {
                LoadTimingIfNeeded();
                return timing;
            }
            set => timing = value;
        }

        public float SelectedDelay =>
            ResolveDelay(Timing != null ? Timing.SelectedDelay : DefaultSelectedDelay);

        public float RevealedDelay =>
            ResolveDelay(Timing != null ? Timing.RevealedDelay : DefaultRevealedDelay);

        public float RevealDelay => RevealedDelay;

        public float SuitCheckDelay =>
            ResolveDelay(Timing != null ? Timing.SuitCheckDelay : DefaultSuitCheckDelay);

        public float ValueCheckDelay =>
            ResolveDelay(Timing != null ? Timing.ValueCheckDelay : DefaultValueCheckDelay);

        public float ResultDelay =>
            ResolveDelay(Timing != null ? Timing.ResultDelay : DefaultResultDelay);

        public float NextTurnDelay =>
            ResolveDelay(Timing != null ? Timing.NextTurnDelay : DefaultNextTurnDelay);

        public bool IsResolving { get; private set; }
        public ResolutionStep ResolutionStep { get; private set; }
        public Card? SelectedPlayerCard { get; private set; }

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
            _session = new RunSession(enemies, Encounter.DefaultMaxTurns);
        }

        public void SelectDeck(int index)
        {
            EnsureSession();
            if (index < 0 || index >= _decks.Count)
            {
                throw new System.ArgumentOutOfRangeException(nameof(index));
            }

            StopResolution();
            _lastPlay = null;
            SelectedPlayerCard = null;
            ResolutionStep = ResolutionStep.None;
            _session.SelectDeck(_decks[index].Cards);
            GameplayEventBus.Raise(GameplayEvent.TurnStart);
        }

        public PlayResult PlayFromHand(int handIndex)
        {
            EnsureSession();
            PlayResult result = _session.PlayFromHand(handIndex);
            _lastPlay = result;
            return result;
        }

        public void PlayFromHandAnimated(int handIndex)
        {
            if (IsResolving)
            {
                return;
            }

            EnsureSession();
            if (_resolution != null)
            {
                StopCoroutine(_resolution);
            }

            _resolution = StartCoroutine(ResolveRound(handIndex));
        }

        public void Restart()
        {
            StopResolution();
            _lastPlay = null;
            SelectedPlayerCard = null;
            ResolutionStep = ResolutionStep.None;
            LoadContentIfNeeded();
            IReadOnlyList<NamedEnemy> enemies = content != null ? content.ToEnemies() : DefaultCatalog.Enemies;
            _decks = content != null ? content.ToDecks() : DefaultCatalog.Decks;
            _session = new RunSession(enemies, Encounter.DefaultMaxTurns);
        }

        private IEnumerator ResolveRound(int handIndex)
        {
            IsResolving = true;
            SelectedPlayerCard = _session.Deck.Hand[handIndex];
            ResolutionStep = ResolutionStep.Selected;
            GameplayEventBus.Raise(GameplayEvent.CardSelected);

            yield return new WaitForSeconds(SelectedDelay);

            PlayResult result = PlayFromHand(handIndex);
            ResolutionStep = ResolutionStep.Revealed;
            GameplayEventBus.Raise(GameplayEvent.EnemyCardSelected);
            GameplayEventBus.Raise(GameplayEvent.CardsRevealed);

            yield return new WaitForSeconds(RevealedDelay);

            ResolutionStep = ResolutionStep.SuitChecked;
            GameplayEventBus.Raise(GameplayEvent.SuitChecked);

            yield return new WaitForSeconds(SuitCheckDelay);

            ResolutionStep = ResolutionStep.ValueChecked;
            GameplayEventBus.Raise(GameplayEvent.ValueChecked);

            yield return new WaitForSeconds(ValueCheckDelay);

            ResolutionStep = ResolutionStep.ResultShown;
            RaiseOutcomeEvents(result);
            GameplayEventBus.Raise(GameplayEvent.CardDiscarded);

            yield return new WaitForSeconds(ResultDelay);

            GameplayEventBus.Raise(GameplayEvent.TurnEnd);

            if (result.RunEnded)
            {
                GameplayEventBus.Raise(
                    result.Phase == RunPhase.Victory ? GameplayEvent.GameWin : GameplayEvent.GameLose);
            }
            else
            {
                yield return new WaitForSeconds(NextTurnDelay);
                GameplayEventBus.Raise(GameplayEvent.TurnStart);
            }

            ResolutionStep = ResolutionStep.None;
            IsResolving = false;
            _resolution = null;
        }

        private static void RaiseOutcomeEvents(PlayResult result)
        {
            switch (result.Round.Resolution.Outcome)
            {
                case RoundOutcome.PlayerWin:
                    GameplayEventBus.Raise(GameplayEvent.PlayerWin);
                    break;
                case RoundOutcome.EnemyWin:
                    GameplayEventBus.Raise(GameplayEvent.PlayerLose);
                    break;
                default:
                    GameplayEventBus.Raise(GameplayEvent.Draw);
                    break;
            }
        }

        private void StopResolution()
        {
            if (_resolution != null)
            {
                StopCoroutine(_resolution);
                _resolution = null;
            }

            IsResolving = false;
            ResolutionStep = ResolutionStep.None;
        }

        private void LoadContentIfNeeded()
        {
#if UNITY_EDITOR
            if (content == null)
            {
                content = UnityEditor.AssetDatabase.LoadAssetAtPath<GameContent>(
                    "Assets/_Game/Content/GameContent.asset");
            }
#endif
            LoadTimingIfNeeded();
        }

        private void LoadTimingIfNeeded()
        {
            if (timing != null)
            {
                return;
            }

#if UNITY_EDITOR
            timing = UnityEditor.AssetDatabase.LoadAssetAtPath<BattleTiming>(DefaultTimingAssetPath);
#endif
        }

        private static float ResolveDelay(float seconds)
        {
            return Mathf.Max(0f, seconds);
        }
    }
}
