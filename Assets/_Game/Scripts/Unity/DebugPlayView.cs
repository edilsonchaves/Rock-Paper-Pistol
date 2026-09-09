using RockPaperPistol.Core;
using UnityEngine;

namespace RockPaperPistol.Unity
{
    public sealed class DebugPlayView : MonoBehaviour
    {
        private GameSessionDriver _driver;
        private AudioManager _audio;
        private Vector2 _scroll;

        private void Awake()
        {
            _driver = GetComponent<GameSessionDriver>();
            if (_driver == null)
            {
                _driver = gameObject.AddComponent<GameSessionDriver>();
            }

            _audio = GetComponent<AudioManager>();
            if (_audio == null)
            {
                _audio = gameObject.AddComponent<AudioManager>();
            }
        }

        private void OnGUI()
        {
#if UNITY_2023_1_OR_NEWER
            if (FindFirstObjectByType<Battle.BattleBoard>() != null)
#else
            if (FindObjectOfType<Battle.BattleBoard>() != null)
#endif
            {
                return;
            }

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
            _scroll.y -= GameInput.MouseScrollY;
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
            GUILayout.Label("Cada lado saca 8 cartas do baralho base e soma a própria Pistola.");
            GUILayout.Label($"Turnos por encontro: {Encounter.DefaultMaxTurns} (MAX_TURNS configurável).");
            GUILayout.Label("Ordem dos oponentes: Estátua de Pedra → Múmia → Pirata.");
            GUILayout.Space(8);

            var decks = _driver.Decks;
            for (int i = 0; i < decks.Count; i++)
            {
                NamedDeck deck = decks[i];
                if (GameInput.ImguiButton($"{deck.Name}\n{FormatCards(deck.Cards)}", GUILayout.Height(56)))
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
                $"Tendência do oponente: {Card.SuitName(session.CurrentEnemy.PreferredSuit)} (35% neste naipe, 65% aleatório)");
            GUILayout.Label(
                $"Placar  {DefaultCatalog.PlayerName} {encounter.PlayerScore}  ×  {encounter.EnemyScore}  {enemy}    " +
                $"Turno {encounter.RoundsPlayed + 1}/{encounter.MaxTurns}    " +
                $"Este turno vale {encounter.CurrentStake} ponto(s)");
            GUILayout.Label(
                $"Cartas disponíveis: você {session.Deck.HandCount}  ·  inimigo {session.EnemyDeck.HandCount}    " +
                $"De fora: você {FormatCards(session.Deck.Excluded)}  ·  inimigo {FormatCards(session.EnemyDeck.Excluded)}");

            DrawResolution();
            GUILayout.Space(8);

            if (_driver.IsResolving)
            {
                GUILayout.Label("Resolvendo o turno...");
                GUILayout.Label("Sua mão (indisponível durante a resolução): " + FormatCards(session.Deck.Hand));
            }
            else
            {
                GUILayout.Label("Sua mão (8 básicas + Pistola) — clique para jogar:");
                for (int i = 0; i < session.Deck.Hand.Count; i++)
                {
                    Card card = session.Deck.Hand[i];
                    if (GameInput.ImguiButton(card.ToString(), GUILayout.Height(36)))
                    {
                        _driver.PlayFromHandAnimated(i);
                    }
                }
            }

            GUILayout.Space(8);
            GUILayout.Label($"Descarte do Pistoleiro ({session.Deck.DiscardCount}): " + FormatCards(session.Deck.Discard));
            GUILayout.Label($"Descarte do inimigo ({session.EnemyDeck.DiscardCount}): " + FormatCards(session.EnemyDeck.Discard));
            DrawAudioHook();
        }

        private void DrawResolution()
        {
            if (_driver.ResolutionStep == ResolutionStep.Selected && _driver.SelectedPlayerCard.HasValue)
            {
                GUILayout.Box($"Carta escolhida: {DefaultCatalog.PlayerName} {_driver.SelectedPlayerCard.Value}");
                return;
            }

            if (!_driver.LastPlay.HasValue)
            {
                GUILayout.Label("O oponente revela a carta só depois da jogada do Pistoleiro.");
                return;
            }

            PlayResult play = _driver.LastPlay.Value;
            EncounterRoundResult round = play.Round;
            ResolutionStep step = _driver.ResolutionStep;

            if (step == ResolutionStep.None && !_driver.IsResolving)
            {
                DrawFullResult(play, round);
                return;
            }

            if (step >= ResolutionStep.Revealed)
            {
                GUILayout.Box(
                    $"Cartas reveladas: {DefaultCatalog.PlayerName} {round.PlayerCard}  vs  " +
                    $"{EnemyNameForLastPlay()} {round.EnemyCard}");
            }

            if (step >= ResolutionStep.SuitChecked)
            {
                GUILayout.Box(DescribeSuitCheck(round.Resolution.PlayerSuit, round.Resolution.EnemySuit));
            }

            if (step >= ResolutionStep.ValueChecked)
            {
                GUILayout.Box(
                    $"Valor: {DefaultCatalog.PlayerName} {round.PlayerCard.Value} → {round.Resolution.PlayerAdjusted}    " +
                    $"{EnemyNameForLastPlay()} {round.EnemyCard.Value} → {round.Resolution.EnemyAdjusted}");
            }

            if (step >= ResolutionStep.ResultShown)
            {
                GUILayout.Box(DescribeOutcome(round));
                if (play.EncounterEnded && play.Phase == RunPhase.InEncounter)
                {
                    GUILayout.Box("Oponente derrotado. Baralho restaurado. Próximo oponente.");
                }
            }
        }

        private void DrawFullResult(PlayResult play, EncounterRoundResult round)
        {
            GUILayout.Box(
                $"Último turno: {DefaultCatalog.PlayerName} {round.PlayerCard} ({round.Resolution.PlayerAdjusted})  vs  " +
                $"{EnemyNameForLastPlay()} {round.EnemyCard} ({round.Resolution.EnemyAdjusted})\n{DescribeOutcome(round)}");

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

            if (_driver.LastPlay.HasValue)
            {
                DrawFullResult(_driver.LastPlay.Value, _driver.LastPlay.Value.Round);
            }

            DrawAudioHook();
            GUILayout.Space(8);
            if (GameInput.ImguiButton("Nova run — baralho base de novo", GUILayout.Height(40)))
            {
                _driver.Restart();
            }
        }

        private void DrawAudioHook()
        {
            if (_audio == null || !_audio.LastEvent.HasValue)
            {
                return;
            }

            GUILayout.Label($"Áudio preparado: {_audio.LastEvent.Value}");
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

        private static string DescribeOutcome(EncounterRoundResult round)
        {
            switch (round.Resolution.Outcome)
            {
                case RoundOutcome.PlayerWin:
                    return $"{DefaultCatalog.PlayerName} ganhou (+{round.Resolution.StakeAwarded})";
                case RoundOutcome.EnemyWin:
                    return $"Oponente ganhou (+{round.Resolution.StakeAwarded})";
                default:
                    return "Empate — próximo turno vale +1";
            }
        }

        private static string DescribeSuitCheck(Suit playerSuit, Suit enemySuit)
        {
            if (CardComparer.Beats(playerSuit, enemySuit))
            {
                return $"Naipe: {Card.SuitName(playerSuit)} vence {Card.SuitName(enemySuit)} (+1 para {DefaultCatalog.PlayerName})";
            }

            if (CardComparer.Beats(enemySuit, playerSuit))
            {
                return $"Naipe: {Card.SuitName(enemySuit)} vence {Card.SuitName(playerSuit)} (+1 para o oponente)";
            }

            return "Naipe: iguais, sem bônus";
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
