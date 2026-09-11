using System.Collections.Generic;
using RockPaperPistol.Core;
using RockPaperPistol.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RockPaperPistol.Unity.Battle
{
    public sealed class BattleBoard : MonoBehaviour
    {
        private static readonly Vector3 PlayerSlot = new Vector3(-2.15f, -0.15f, 0f);
        private static readonly Vector3 EnemySlot = new Vector3(2.15f, 0.85f, 0f);
        private static readonly Vector3 OpponentPos = new Vector3(0f, 2.55f, 0f);

        private GameSessionDriver _driver;
        private readonly List<CardView> _hand = new List<CardView>();
        private readonly List<SpriteRenderer> _enemyBacks = new List<SpriteRenderer>();
        private readonly List<SpriteRenderer> _circles = new List<SpriteRenderer>();
        private readonly List<RoundOutcome> _turnFaces = new List<RoundOutcome>();

        private Transform _handRoot;
        private Transform _enemyHandRoot;
        private CardView _playerSlotCard;
        private CardView _enemySlotCard;
        private SpriteRenderer _bubble;
        private TextMeshPro _turnText;
        private TextMeshPro _scoreText;
        private TextMeshPro _bubbleText;
        private TextMeshPro _promptText;
        private TextMeshPro _pauseText;
        private CardView _hover;
        private ResolutionStep _lastStep = (ResolutionStep)(-1);
        private int _lastHandCount = -1;
        private int _lastEnemyCount = -1;
        private int _lastRounds = -1;
        private RunPhase _lastPhase;
        private bool _paused;
        private int _selectedIndex = -1;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
#if UNITY_2023_1_OR_NEWER
            if (FindFirstObjectByType<BattleBoard>() != null)
#else
            if (FindObjectOfType<BattleBoard>() != null)
#endif
            {
                return;
            }

            if (!GameFlow.IsBattleScene(SceneManager.GetActiveScene().name))
            {
                return;
            }

            GameObject root = new GameObject("RockPaperPistol");
            root.AddComponent<GameSessionDriver>();
            root.AddComponent<AudioManager>();
            root.AddComponent<BattleBoard>();
        }

        private void Awake()
        {
            _driver = GetComponent<GameSessionDriver>();
            if (_driver == null)
            {
                _driver = gameObject.AddComponent<GameSessionDriver>();
            }

            if (GetComponent<AudioManager>() == null)
            {
                gameObject.AddComponent<AudioManager>();
            }

            StyleCamera();
            HideBlockingUi();
            BuildStage();
        }

        private void Start()
        {
            BeginPlaytestIfNeeded();
        }

        private void Update()
        {
            if (GameInput.EscapePressed || ClickedWorld(new Vector3(7.2f, 4.2f, 0f), 1.3f, 0.55f))
            {
                TogglePause();
            }

            if (_paused)
            {
                return;
            }

            RunSession session = _driver.Session;
            if (session.Phase == RunPhase.AwaitingDeck)
            {
                BeginPlaytestIfNeeded();
                return;
            }

            if (session.Phase == RunPhase.GameOver || session.Phase == RunPhase.Victory)
            {
                _promptText.gameObject.SetActive(true);
                _promptText.text = session.Phase == RunPhase.Victory
                    ? "Vitória. Clique para voltar ao menu."
                    : "Derrota. Clique para voltar ao menu.";
                if (GameInput.LeftClickPressed)
                {
                    Time.timeScale = 1f;
                    GameFlow.GoToMenu();
                }

                return;
            }

            _promptText.gameObject.SetActive(false);
            RefreshIfNeeded(session);
            HandleHoverAndClick();
        }

        private void RefreshIfNeeded(RunSession session)
        {
            Encounter encounter = session.CurrentEncounter;
            int rounds = encounter != null ? encounter.RoundsPlayed : 0;
            bool stepChanged = _driver.ResolutionStep != _lastStep;
            bool handChanged = session.Deck.HandCount != _lastHandCount;
            bool enemyChanged = session.EnemyDeck.HandCount != _lastEnemyCount;
            bool phaseChanged = session.Phase != _lastPhase;
            bool roundChanged = rounds != _lastRounds;

            if (!stepChanged && !handChanged && !enemyChanged && !phaseChanged && !roundChanged)
            {
                return;
            }

            if (roundChanged && rounds == 0)
            {
                _turnFaces.Clear();
            }

            if (stepChanged && _driver.ResolutionStep == ResolutionStep.ResultShown && _driver.LastPlay.HasValue)
            {
                _turnFaces.Add(_driver.LastPlay.Value.Round.Resolution.Outcome);
            }

            _lastStep = _driver.ResolutionStep;
            _lastHandCount = session.Deck.HandCount;
            _lastEnemyCount = session.EnemyDeck.HandCount;
            _lastPhase = session.Phase;
            _lastRounds = rounds;

            LayoutEncounter(session);
        }

        private void LayoutEncounter(RunSession session)
        {
            Encounter encounter = session.CurrentEncounter;
            int turn = encounter != null ? Mathf.Min(encounter.RoundsPlayed + 1, encounter.MaxTurns) : 1;
            int max = encounter != null ? encounter.MaxTurns : Encounter.DefaultMaxTurns;
            _turnText.text = $"Turno {turn}/{max}";
            if (encounter != null)
            {
                _scoreText.text = $"{encounter.PlayerScore} x {encounter.EnemyScore}  {session.CurrentEnemyName}";
            }

            RefreshCircles();
            LayoutEnemyBacks(session.EnemyDeck.HandCount);
            LayoutHand(session);
            LayoutTable(session);
            UpdateBubble();
        }

        private void LayoutHand(RunSession session)
        {
            ClearViews(_hand, _handRoot);
            IReadOnlyList<Card> cards = session.Deck.Hand;
            int turnForHand = EncounterTurnBeforePlay(session);
            int count = cards.Count;
            for (int i = 0; i < count; i++)
            {
                if (_driver.ResolutionStep == ResolutionStep.Selected && i == _selectedIndex)
                {
                    continue;
                }

                CardView view = CardView.Create(_handRoot, "HandCard");
                view.HandIndex = i;
                view.Interactable = !_driver.IsResolving;
                view.Bind(cards[i], cards[i].Suit, DisplayValue(cards[i], turnForHand), false, true);
                _hand.Add(view);
            }

            for (int i = 0; i < _hand.Count; i++)
            {
                float t = _hand.Count <= 1 ? 0.5f : i / (float)(_hand.Count - 1);
                float x = Mathf.Lerp(-6.2f, 6.2f, t);
                float angle = Mathf.Lerp(12f, -12f, t);
                _hand[i].transform.localPosition = new Vector3(x, -3.2f, 0f);
                _hand[i].transform.localRotation = Quaternion.Euler(0f, 0f, angle);
                _hand[i].transform.localScale = Vector3.one * 0.82f;
            }

            if (_driver.ResolutionStep == ResolutionStep.None)
            {
                _selectedIndex = -1;
            }
        }

        private void LayoutTable(RunSession session)
        {
            ResolutionStep step = _driver.ResolutionStep;
            if (step == ResolutionStep.None || !_driver.SelectedPlayerCard.HasValue && !_driver.LastPlay.HasValue)
            {
                ClearTableCards();
                return;
            }

            Card playerCard = _driver.LastPlay.HasValue && step >= ResolutionStep.Revealed
                ? _driver.LastPlay.Value.Round.PlayerCard
                : _driver.SelectedPlayerCard.Value;
            Suit playerSuit = playerCard.Suit;
                int playerValue = DisplayValue(playerCard, EncounterTurnAfterPlay(session));
            bool plusOne = false;

            if (_driver.LastPlay.HasValue && step >= ResolutionStep.SuitChecked)
            {
                playerSuit = _driver.LastPlay.Value.Round.Resolution.PlayerSuit;
            }

            if (_driver.LastPlay.HasValue && step >= ResolutionStep.ValueChecked)
            {
                playerValue = CardComparer.ResolveValue(
                    playerCard,
                    _driver.LastPlay.Value.Round.Resolution.EnemySuit,
                    EncounterTurnAfterPlay(session));
                plusOne = _driver.LastPlay.Value.Round.Resolution.PlayerAdjusted > playerValue;
            }

            EnsureSlot(ref _playerSlotCard, "PlayerSlotCard", PlayerSlot, 1f);
            _playerSlotCard.Interactable = false;
            _playerSlotCard.Bind(playerCard, playerSuit, playerValue, plusOne, true);

            if (step >= ResolutionStep.Revealed && _driver.LastPlay.HasValue)
            {
                Card enemyCard = _driver.LastPlay.Value.Round.EnemyCard;
                Suit enemySuit = step >= ResolutionStep.SuitChecked
                    ? _driver.LastPlay.Value.Round.Resolution.EnemySuit
                    : enemyCard.Suit;
                int enemyValue = DisplayValue(enemyCard, EncounterTurnAfterPlay(session));
                bool enemyPlus = false;
                if (step >= ResolutionStep.ValueChecked)
                {
                    enemyValue = CardComparer.ResolveValue(
                        enemyCard,
                        _driver.LastPlay.Value.Round.Resolution.PlayerSuit,
                        EncounterTurnAfterPlay(session));
                    enemyPlus = _driver.LastPlay.Value.Round.Resolution.EnemyAdjusted > enemyValue;
                }

                EnsureSlot(ref _enemySlotCard, "EnemySlotCard", EnemySlot, 0.78f);
                _enemySlotCard.Interactable = false;
                _enemySlotCard.Bind(enemyCard, enemySuit, enemyValue, enemyPlus, true);
            }
            else
            {
                EnsureSlot(ref _enemySlotCard, "EnemySlotCard", EnemySlot, 0.78f);
                _enemySlotCard.Interactable = false;
                _enemySlotCard.Bind(new Card(Suit.Rock, 1), Suit.Rock, 1, false, false);
            }
        }

        private void LayoutEnemyBacks(int remaining)
        {
            while (_enemyBacks.Count < remaining)
            {
                GameObject go = new GameObject("EnemyBack");
                go.transform.SetParent(_enemyHandRoot, false);
                SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
                renderer.sprite = PlaceholderArt.CardBack();
                renderer.sortingOrder = 4;
                PlaceholderArt.ApplyVisibleMaterial(renderer);
                _enemyBacks.Add(renderer);
            }

            for (int i = 0; i < _enemyBacks.Count; i++)
            {
                bool on = i < remaining && _driver.ResolutionStep == ResolutionStep.None;
                _enemyBacks[i].enabled = on;
                if (!on)
                {
                    continue;
                }

                float t = remaining == 1 ? 0.5f : i / (float)(remaining - 1);
                float x = Mathf.Lerp(-1.6f, 1.6f, t);
                _enemyBacks[i].transform.localPosition = new Vector3(x, 1.55f, 0f);
                _enemyBacks[i].transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(10f, -10f, t));
                _enemyBacks[i].transform.localScale = Vector3.one * 0.38f;
            }
        }

        private void RefreshCircles()
        {
            for (int i = 0; i < _circles.Count; i++)
            {
                if (i >= _turnFaces.Count)
                {
                    _circles[i].sprite = PlaceholderArt.CircleEmpty();
                    continue;
                }

                switch (_turnFaces[i])
                {
                    case RoundOutcome.PlayerWin:
                        _circles[i].sprite = PlaceholderArt.FacePlayer();
                        break;
                    case RoundOutcome.EnemyWin:
                        _circles[i].sprite = PlaceholderArt.FaceEnemy();
                        break;
                    default:
                        _circles[i].sprite = PlaceholderArt.CircleDraw();
                        break;
                }
            }
        }

        private void UpdateBubble()
        {
            bool show = _driver.ResolutionStep == ResolutionStep.ResultShown && _driver.LastPlay.HasValue;
            _bubble.enabled = show;
            _bubbleText.gameObject.SetActive(show);
            if (!show)
            {
                return;
            }

            switch (_driver.LastPlay.Value.Round.Resolution.Outcome)
            {
                case RoundOutcome.PlayerWin:
                    _bubbleText.text = "Boa jogada!";
                    break;
                case RoundOutcome.EnemyWin:
                    _bubbleText.text = "Hmm...";
                    break;
                default:
                    _bubbleText.text = "Empate.";
                    break;
            }
        }

        private void HandleHoverAndClick()
        {
            CardView hit = RaycastCard();
            if (_hover != hit)
            {
                if (_hover != null)
                {
                    _hover.SetHover(false);
                }

                _hover = hit;
                if (_hover != null)
                {
                    _hover.SetHover(true);
                }
            }

            if (hit == null || !GameInput.LeftClickPressed || _driver.IsResolving)
            {
                return;
            }

            _selectedIndex = hit.HandIndex;
            _driver.PlayFromHandAnimated(hit.HandIndex);
        }

        private CardView RaycastCard()
        {
            Camera camera = Camera.main;
            if (camera == null)
            {
                return null;
            }

            Vector3 mouse = GameInput.MouseScreenPosition;
            mouse.z = Mathf.Abs(camera.transform.position.z);
            Vector3 world = camera.ScreenToWorldPoint(mouse);
            RaycastHit2D hit = Physics2D.Raycast(world, Vector2.zero);
            if (hit.collider == null)
            {
                return null;
            }

            return hit.collider.GetComponent<CardView>();
        }

        private bool ClickedWorld(Vector3 center, float w, float h)
        {
            if (!GameInput.LeftClickPressed || Camera.main == null)
            {
                return false;
            }

            Vector3 mouse = GameInput.MouseScreenPosition;
            mouse.z = Mathf.Abs(Camera.main.transform.position.z);
            Vector3 world = Camera.main.ScreenToWorldPoint(mouse);
            return Mathf.Abs(world.x - center.x) <= w && Mathf.Abs(world.y - center.y) <= h;
        }

        private void TogglePause()
        {
            _paused = !_paused;
            Time.timeScale = _paused ? 0f : 1f;
            _pauseText.text = _paused ? "Retomar" : "Pause";
        }

        private void BuildStage()
        {
            CreateSprite("Table", PlaceholderArt.Table(), new Vector3(0f, -0.35f, 0f), new Vector3(3.6f, 3.2f, 1f), 0);
            CreateSprite("PlayerSlot", PlaceholderArt.Slot(), PlayerSlot, Vector3.one, 1);
            CreateSprite("EnemySlot", PlaceholderArt.Slot(), EnemySlot, Vector3.one * 0.82f, 1);
            CreateSprite("Opponent", PlaceholderArt.Opponent(), OpponentPos, Vector3.one * 1.15f, 3);

            _handRoot = new GameObject("Hand").transform;
            _handRoot.SetParent(transform, false);
            _enemyHandRoot = new GameObject("EnemyHand").transform;
            _enemyHandRoot.SetParent(transform, false);

            _bubble = CreateSprite("Bubble", PlaceholderArt.Bubble(), new Vector3(3.4f, 3.15f, 0f), Vector3.one * 1.3f, 6);
            _bubble.enabled = false;

            for (int i = 0; i < 7; i++)
            {
                SpriteRenderer circle = CreateSprite(
                    "Circle" + i,
                    PlaceholderArt.CircleEmpty(),
                    new Vector3(-7.3f + i * 0.48f, 4.15f, 0f),
                    Vector3.one,
                    8);
                _circles.Add(circle);
            }

            _turnText = CreateText("TurnText", new Vector3(-6.4f, 4.65f, 0f), 5.2f, TextAlignmentOptions.MidlineLeft);
            _turnText.text = "Turno 1/7";
            _scoreText = CreateText("ScoreText", new Vector3(-6.4f, 3.72f, 0f), 3.6f, TextAlignmentOptions.MidlineLeft);
            _bubbleText = CreateText("BubbleText", new Vector3(3.4f, 3.15f, 0f), 3.4f, TextAlignmentOptions.Center);
            _bubbleText.color = Color.black;
            _bubbleText.gameObject.SetActive(false);
            _promptText = CreateText("Prompt", new Vector3(0f, 0.2f, 0f), 5.5f, TextAlignmentOptions.Center);
            _pauseText = CreateText("Pause", new Vector3(7.2f, 4.2f, 0f), 4f, TextAlignmentOptions.Center);
            _pauseText.text = "Pause";
        }

        private void StyleCamera()
        {
            Camera camera = Camera.main;
            if (camera == null)
            {
                return;
            }

            camera.orthographic = true;
            camera.orthographicSize = 5.2f;
            camera.backgroundColor = new Color(0.16f, 0.16f, 0.17f);
            camera.clearFlags = CameraClearFlags.SolidColor;
        }

        private void EnsureSlot(ref CardView view, string name, Vector3 position, float scale)
        {
            if (view == null)
            {
                view = CardView.Create(transform, name);
            }

            view.transform.position = position;
            view.transform.rotation = Quaternion.identity;
            view.transform.localScale = Vector3.one * scale;
            view.SetHover(false);
            view.SetColliderEnabled(false);
        }

        private void ClearTableCards()
        {
            if (_playerSlotCard != null)
            {
                Destroy(_playerSlotCard.gameObject);
                _playerSlotCard = null;
            }

            if (_enemySlotCard != null)
            {
                Destroy(_enemySlotCard.gameObject);
                _enemySlotCard = null;
            }
        }

        private static void ClearViews(List<CardView> views, Transform root)
        {
            for (int i = 0; i < views.Count; i++)
            {
                if (views[i] != null)
                {
                    Destroy(views[i].gameObject);
                }
            }

            views.Clear();
            if (root == null)
            {
                return;
            }

            for (int i = root.childCount - 1; i >= 0; i--)
            {
                Destroy(root.GetChild(i).gameObject);
            }
        }

        private SpriteRenderer CreateSprite(string name, Sprite sprite, Vector3 position, Vector3 scale, int order)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(transform, false);
            go.transform.position = position;
            go.transform.localScale = scale;
            SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = order;
            PlaceholderArt.ApplyVisibleMaterial(renderer);
            return renderer;
        }

        private void BeginPlaytestIfNeeded()
        {
            if (_driver == null || _driver.Session.Phase != RunPhase.AwaitingDeck)
            {
                return;
            }

            _promptText.gameObject.SetActive(false);
            _driver.SelectDeck(0);
            _turnFaces.Clear();
            _lastHandCount = -1;
            _lastEnemyCount = -1;
            _lastRounds = -1;
            _lastStep = (ResolutionStep)(-1);
            RefreshIfNeeded(_driver.Session);
        }

        private static void HideBlockingUi()
        {
            GameObject dialog = GameObject.Find("DialogSystem");
            if (dialog != null)
            {
                dialog.SetActive(false);
            }
        }

        private TextMeshPro CreateText(string name, Vector3 position, float fontSize, TextAlignmentOptions align)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(transform, false);
            go.transform.position = position;
            TextMeshPro tmp = go.AddComponent<TextMeshPro>();
            tmp.fontSize = fontSize;
            tmp.alignment = align;
            tmp.color = Color.white;
            tmp.fontStyle = FontStyles.Bold;
            tmp.enableWordWrapping = false;
            tmp.overflowMode = TextOverflowModes.Overflow;
            tmp.rectTransform.sizeDelta = new Vector2(8f, 1.2f);
            tmp.sortingOrder = 20;
            return tmp;
        }

        private static int EncounterTurnBeforePlay(RunSession session)
        {
            Encounter encounter = session.CurrentEncounter;
            return encounter == null ? 1 : encounter.RoundsPlayed + 1;
        }

        private static int EncounterTurnAfterPlay(RunSession session)
        {
            Encounter encounter = session.CurrentEncounter;
            if (encounter == null)
            {
                return 1;
            }

            return encounter.RoundsPlayed < 1 ? 1 : encounter.RoundsPlayed;
        }

        private static int DisplayValue(Card card, int turn)
        {
            if (card.Pistol == PistolId.Mumia)
            {
                return CardComparer.MumiaValueForTurn(turn);
            }

            return card.Value;
        }
    }
}
