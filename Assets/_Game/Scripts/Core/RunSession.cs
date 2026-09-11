using System;
using System.Collections.Generic;

namespace RockPaperPistol.Core
{
    public enum RunPhase
    {
        AwaitingDeck,
        InEncounter,
        GameOver,
        Victory
    }

    public readonly struct PlayResult
    {
        public PlayResult(
            EncounterRoundResult round,
            bool encounterEnded,
            bool runEnded,
            RunPhase phase,
            int enemyIndex,
            int enemiesDefeated)
        {
            Round = round;
            EncounterEnded = encounterEnded;
            RunEnded = runEnded;
            Phase = phase;
            EnemyIndex = enemyIndex;
            EnemiesDefeated = enemiesDefeated;
        }

        public EncounterRoundResult Round { get; }
        public bool EncounterEnded { get; }
        public bool RunEnded { get; }
        public RunPhase Phase { get; }
        public int EnemyIndex { get; }
        public int EnemiesDefeated { get; }
    }

    public sealed class RunSession
    {
        public const int EnemyCount = 3;

        private readonly NamedEnemy[] _enemies;
        private readonly DeckRuntime _deck = new DeckRuntime();
        private readonly DeckRuntime _enemyDeck = new DeckRuntime();
        private readonly PlayerLoadout _loadout;
        private readonly int _maxTurns;
        private Random _rng = new Random();

        public RunSession(
            IReadOnlyList<NamedEnemy> enemies,
            int maxTurns = Encounter.DefaultMaxTurns,
            PlayerLoadout loadout = null)
        {
            if (enemies == null)
            {
                throw new ArgumentNullException(nameof(enemies));
            }

            if (enemies.Count != EnemyCount)
            {
                throw new ArgumentException(
                    $"A run precisa de exatamente {EnemyCount} inimigos.",
                    nameof(enemies));
            }

            if (maxTurns < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxTurns), maxTurns, "MAX_TURNS deve ser pelo menos 1.");
            }

            _enemies = new NamedEnemy[EnemyCount];
            for (int i = 0; i < EnemyCount; i++)
            {
                _enemies[i] = enemies[i];
            }

            _maxTurns = maxTurns;
            _loadout = loadout ?? PlayerLoadout.Default;
            Phase = RunPhase.AwaitingDeck;
        }

        public RunSession() : this(DefaultCatalog.Enemies)
        {
        }

        public RunPhase Phase { get; private set; }
        public DeckRuntime Deck => _deck;
        public DeckRuntime EnemyDeck => _enemyDeck;
        public Encounter CurrentEncounter { get; private set; }
        public int MaxTurns => _maxTurns;
        public int EnemyIndex { get; private set; }
        public int EnemiesDefeated { get; private set; }

        public NamedEnemy CurrentEnemy =>
            EnemyIndex >= 0 && EnemyIndex < _enemies.Length
                ? _enemies[EnemyIndex]
                : default;

        public string CurrentEnemyName => CurrentEnemy.Name;

        public IReadOnlyList<NamedEnemy> Enemies => _enemies;

        public PlayerLoadout Loadout => _loadout;

        public void SelectDeck(IReadOnlyList<Card> composition, Random rng = null)
        {
            if (composition == null)
            {
                throw new ArgumentNullException(nameof(composition));
            }

            if (rng != null)
            {
                _rng = rng;
                _deck.SetRandom(rng);
                _enemyDeck.SetRandom(rng);
            }

            _deck.ResetFrom(composition);
            EnemyIndex = 0;
            EnemiesDefeated = 0;
            StartCurrentEncounter();
        }

        public PlayResult PlayFromHand(int handIndex)
        {
            if (Phase != RunPhase.InEncounter || CurrentEncounter == null)
            {
                throw new InvalidOperationException("Não há encontro em andamento.");
            }

            Card played = _deck.Play(handIndex);
            Card enemyPlayed = _enemyDeck.Play(ChooseEnemyHandIndex());
            EncounterRoundResult round = CurrentEncounter.PlayRound(played, enemyPlayed);

            if (!CurrentEncounter.IsFinished)
            {
                _deck.DrawUpTo(DeckRuntime.DefaultHandSize);
                return new PlayResult(
                    round,
                    encounterEnded: false,
                    runEnded: false,
                    Phase,
                    EnemyIndex,
                    EnemiesDefeated);
            }

            if (CurrentEncounter.Status == EncounterStatus.PlayerWon)
            {
                EnemiesDefeated += 1;
                if (EnemiesDefeated >= EnemyCount)
                {
                    Phase = RunPhase.Victory;
                    return new PlayResult(round, true, true, Phase, EnemyIndex, EnemiesDefeated);
                }

                EnemyIndex += 1;
                StartCurrentEncounter();
                return new PlayResult(round, true, false, Phase, EnemyIndex, EnemiesDefeated);
            }

            Phase = RunPhase.GameOver;
            return new PlayResult(round, true, true, Phase, EnemyIndex, EnemiesDefeated);
        }

        private void StartCurrentEncounter()
        {
            _deck.PrepareMatchHand(_loadout.Pistol);
            _enemyDeck.ResetFrom(_enemies[EnemyIndex].Sequence);
            _enemyDeck.PrepareMatchHand(_enemies[EnemyIndex].Pistol);
            CurrentEncounter = new Encounter(_maxTurns);
            Phase = RunPhase.InEncounter;
        }

        private int ChooseEnemyHandIndex()
        {
            NamedEnemy enemy = CurrentEnemy;
            int turn = CurrentEncounter != null ? CurrentEncounter.RoundsPlayed + 1 : 1;
            return EnemyCardPicker.ChooseHandIndex(
                _enemyDeck.Hand,
                enemy.PreferredSuit,
                turn,
                _maxTurns,
                enemy.PistolAvailableFromTurn,
                _rng);
        }
    }
}
