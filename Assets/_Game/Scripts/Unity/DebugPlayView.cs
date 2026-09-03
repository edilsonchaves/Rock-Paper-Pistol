using RockPaperPistol.Core;
using UnityEngine;

namespace RockPaperPistol.Unity
{
    public sealed class DebugPlayView : MonoBehaviour
    {
        private GameSessionDriver _driver;
        private Vector2 _scroll;

        private void Awake()
        {
            _driver = GetComponent<GameSessionDriver>();
            if (_driver == null)
            {
                _driver = gameObject.AddComponent<GameSessionDriver>();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void BootstrapEmptyScene()
        {
#if UNITY_2023_1_OR_NEWER
            if (FindFirstObjectByType<DebugPlayView>() != null)
#else
            if (FindObjectOfType<DebugPlayView>() != null)
#endif
            {
                return;
            }

            GameObject root = new GameObject("RockPaperPistol");
            root.AddComponent<GameSessionDriver>();
            root.AddComponent<DebugPlayView>();
        }

        private void OnGUI()
        {
            if (_driver == null)
            {
                return;
            }

            GUIStyle box = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.UpperLeft,
                wordWrap = true,
                fontSize = 14,
                padding = new RectOffset(12, 12, 12, 12)
            };

            GUILayout.BeginArea(new Rect(16, 16, Screen.width - 32, Screen.height - 32), box);
            _scroll = GUILayout.BeginScrollView(_scroll);

            RunSession session = _driver.Session;
            GUILayout.Label("Rock Paper Pistol — playtest da mecânica", Header());
            GUILayout.Space(8);

            switch (session.Phase)
            {
                case RunPhase.AwaitingDeck:
                    DrawDeckSelect();
                    break;
                case RunPhase.InEncounter:
                    DrawEncounter(session);
                    break;
                case RunPhase.GameOver:
                    DrawEnd($"Game Over — {session.CurrentEnemyName} venceu o Pistoleiro.", session);
                    break;
                case RunPhase.Victory:
                    DrawEnd("Vitória — o Pistoleiro derrotou a Estátua de Pedra, a Múmia e o Pirata.", session);
                    break;
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void DrawDeckSelect()
        {
            GUILayout.Label("Pistoleiro: escolha o baralho. Depois disso ele não muda.");
            GUILayout.Label("Ordem dos oponentes: Estátua de Pedra (defensivo) → Múmia (defensivo) → Pirata (agressivo).");
            GUILayout.Space(8);

            var decks = _driver.Decks;
            for (int i = 0; i < decks.Count; i++)
            {
                NamedDeck deck = decks[i];
                if (GUILayout.Button($"{deck.Name}\n{FormatCards(deck.Cards)}", GUILayout.Height(56)))
                {
                    _driver.SelectDeck(i);
                }
            }
        }

        private void DrawEncounter(RunSession session)
        {
            Encounter encounter = session.CurrentEncounter;
            string enemy = session.CurrentEnemyName;
            string style = EnemyBehaviorText.Label(session.CurrentEnemy.Behavior);
            GUILayout.Label($"{DefaultCatalog.PlayerName} vs {enemy} ({style})  —  {session.EnemyIndex + 1}/3", Header());
            GUILayout.Label(
                $"Placar  {DefaultCatalog.PlayerName} {encounter.PlayerScore}  ×  {encounter.EnemyScore}  {enemy}    " +
                $"Round {encounter.RoundsPlayed + 1}/{Encounter.RoundsPerEncounter}    " +
                $"Este round vale {encounter.CurrentStake} ponto(s)");

            DrawLastPlay();
            GUILayout.Space(8);
            GUILayout.Label("Sua mão — clique para jogar:");

            for (int i = 0; i < session.Deck.Hand.Count; i++)
            {
                Card card = session.Deck.Hand[i];
                if (GUILayout.Button(card.ToString(), GUILayout.Height(36)))
                {
                    _driver.PlayFromHand(i);
                }
            }

            GUILayout.Space(8);
            GUILayout.Label($"Compra: {session.Deck.DrawCount}   Descarte: {session.Deck.DiscardCount}");
            GUILayout.Label("Descarte: " + FormatCards(session.Deck.Discard));
        }

        private void DrawLastPlay()
        {
            if (!_driver.LastPlay.HasValue)
            {
                GUILayout.Label("O oponente revela a carta só depois da jogada do Pistoleiro.");
                return;
            }

            PlayResult play = _driver.LastPlay.Value;
            EncounterRoundResult round = play.Round;
            string outcome;
            switch (round.Resolution.Outcome)
            {
                case RoundOutcome.PlayerWin:
                    outcome = $"{DefaultCatalog.PlayerName} ganhou (+{round.Resolution.StakeAwarded})";
                    break;
                case RoundOutcome.EnemyWin:
                    outcome = $"{EnemyNameForLastPlay()} ganhou (+{round.Resolution.StakeAwarded})";
                    break;
                default:
                    outcome = "Empate — próximo round vale +1";
                    break;
            }

            GUILayout.Box(
                $"Último round: {DefaultCatalog.PlayerName} {round.PlayerCard} ({round.Resolution.PlayerAdjusted})  vs  " +
                $"{EnemyNameForLastPlay()} {round.EnemyCard} ({round.Resolution.EnemyAdjusted})\n{outcome}");

            if (play.EncounterEnded && play.Phase == RunPhase.InEncounter)
            {
                GUILayout.Box("Oponente derrotado. Baralho restaurado. Próximo oponente.");
            }
        }

        private void DrawEnd(string title, RunSession session)
        {
            GUILayout.Label(title, Header());
            if (session.CurrentEncounter != null)
            {
                GUILayout.Label(
                    $"Placar final: {DefaultCatalog.PlayerName} {session.CurrentEncounter.PlayerScore} × " +
                    $"{session.CurrentEncounter.EnemyScore} {session.CurrentEnemyName}");
            }

            DrawLastPlay();
            GUILayout.Space(8);
            if (GUILayout.Button("Nova run — escolher baralho de novo", GUILayout.Height(40)))
            {
                _driver.Restart();
            }
        }

        private string EnemyNameForLastPlay()
        {
            if (_driver.LastPlay.HasValue && _driver.LastPlay.Value.EncounterEnded
                && _driver.Session.Phase == RunPhase.InEncounter
                && _driver.Session.EnemyIndex > 0)
            {
                return _driver.Session.Enemies[_driver.Session.EnemyIndex - 1].Name;
            }

            return _driver.Session.CurrentEnemyName;
        }

        private static string FormatCards(System.Collections.Generic.IReadOnlyList<Card> cards)
        {
            if (cards == null || cards.Count == 0)
            {
                return "—";
            }

            string[] parts = new string[cards.Count];
            for (int i = 0; i < cards.Count; i++)
            {
                parts[i] = cards[i].ToString();
            }

            return string.Join("  ·  ", parts);
        }

        private static GUIStyle Header()
        {
            return new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold
            };
        }
    }
}
