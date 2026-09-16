using System.Collections;
using System.Runtime.Serialization;
using RockPaperPistol.Core;
using RockPaperPistol.Data;
using RockPaperPistol.Enemy;
using RockPaperPistol.Player;
using RockPaperPistol.Unity.Battle;
using RockPaperPistol.Utils;
using RockPaperPistol.Events;
using UnityEngine;

namespace RockPaperPistol.Managers
{
    public class TableManager : MonoBehaviour
    {
        [SerializeField] private GameContent _gameContent;
        [SerializeField] private DeckDefinition _playerDeck;
        private EnemyDefinition _currentEnemy;

        [SerializeField] private EnemyControl _enemyBody;
        [SerializeField] private PlayerControl _playerBody;

        [SerializeField] private Transform _tablePlayerCardPosition;
        [SerializeField] private Transform _tableEnemyCardPosition;

        [SerializeField] private CardView _currentTablePlayerCard;
        [SerializeField] private CardView _currentTableEnemyCard;
        private CardDefinition _playerCardData;
        private CardDefinition _enemyCardData;
        [SerializeField] private RunningSessionMoment _sessionMoment;
        [SerializeField] private int _currentTurn;
        [SerializeField] private int _totalTurn;
        [SerializeField] private int _playerScore;
        [SerializeField] private int _enemyScore;
        [SerializeField] private int _steak;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _sessionMoment = RunningSessionMoment.WaitingPlayer;
            _currentEnemy = _gameContent.Enemies[0];
            _enemyBody.SetupEnemy(_currentEnemy, EnemyThrowCard);
            _playerBody.Setup(_playerDeck, PlayerThrowCard, GameManager.Instance.HeroPistol);
            StartCoroutine(GameSession());
        }

        IEnumerator GameSession()
        {
            _currentTurn = 1;
            _steak = 1;
            GameEvents.UI.onRoundUpdate?.Invoke(_currentTurn, _totalTurn);

            while(_currentTurn <= _totalTurn)
            {
                Debug.Log("Esperando Jogador Jogar");
                yield return new WaitUntil(() => _playerCardData != null);
                Debug.Log("Jogador executou sua jogada");
                Debug.Log("Simulando inimigo pensando a sua jogada");
                yield return new WaitForSeconds(3f);
                Debug.Log("Inimigo executou sua jogada");
                _enemyBody.ThrowCardInSequence();
                yield return new WaitUntil(() => _enemyCardData != null);

                // Comparar resultado do turno atual
                if(_playerCardData is CardNormalDefinition && _enemyCardData is CardNormalDefinition)
                    CompareNormalCardValue();

                if(_playerCardData is CardPistolDefinition || _enemyCardData is CardPistolDefinition)
                    ComparePistolCardValue();

                yield return new WaitForSeconds(3f);
                CleanTable();
                NextTurn();
            }
            yield return null;
            // Aqui irá verificar quem venceu e quem perdeu ou se precisará da rodada de desempate
        }

        private void ComparePistolCardValue()
        {
            if(_playerCardData is CardPistolDefinition && _enemyCardData is CardPistolDefinition)
            {
                DrawTurnResult();
            }
            else
            {
                if(_playerCardData is CardPistolDefinition)
                {
                    var playerWinner = PistolWinner((CardPistolDefinition) _playerCardData, (CardNormalDefinition) _enemyCardData, out bool draw);
                    if (draw)
                    {
                        DrawTurnResult();
                    }
                    else
                    {
                        VictoryTurnResult(playerWinner);
                    }
                }
                else
                {
                    var enemyWinner = PistolWinner((CardPistolDefinition) _enemyCardData, (CardNormalDefinition) _playerCardData, out bool draw);
                    if (draw)
                    {
                        DrawTurnResult();
                    }
                    else
                    {
                       VictoryTurnResult(!enemyWinner);
                    }                    
                }                
            }
        }

        private bool PistolWinner(CardPistolDefinition pistol, CardNormalDefinition simpleCard, out bool draw){
            bool result = false;
            draw = false;
            switch (pistol.Type)
            {
                case PistolId.Pistoleiro:
                    result = GunmanPistol(pistol, simpleCard);
                break;
                case PistolId.Mumia:
                    result = MummyPistol(pistol, simpleCard, out bool resultDraw);
                    draw = resultDraw;
                break;
                case PistolId.Pirata:
                    result = PiratePistol(pistol, simpleCard);
                break;
                case PistolId.Estatua:
                    result = StonePistol(pistol, simpleCard);
                break;
            }
            return result;
        }
        private bool GunmanPistol(CardDefinition pistol, CardNormalDefinition simpleCard)
        {
            return simpleCard.Value < 3;
        }

        private bool MummyPistol(CardPistolDefinition pistol, CardNormalDefinition simpleCard, out bool draw)
        {
            var pistolValue = pistol.Value + Mathf.FloorToInt(_currentTurn / 2);
            draw = false;
            draw = simpleCard.Value == pistolValue;
            return simpleCard.Value < pistolValue;
        }

        private bool StonePistol(CardDefinition pistol, CardNormalDefinition simpleCard)
        {
            return simpleCard.Suit != Suit.Paper;
        }

        private bool PiratePistol(CardDefinition pistol, CardNormalDefinition simpleCard)
        {
            return simpleCard.Suit != Suit.Rock;
        }

        private void CompareNormalCardValue()
        {
                var playerSuit = Suit.Rock;
                if(_playerCardData is CardNormalDefinition)
                {
                    var normalDefinition = (CardNormalDefinition) _playerCardData;
                    playerSuit = normalDefinition.Suit;
                }

                var enemySuit = Suit.Rock;
                if(_enemyCardData is CardNormalDefinition)
                {
                    var normalDefinition = (CardNormalDefinition) _enemyCardData;
                    enemySuit = normalDefinition.Suit;
                }
                _sessionMoment = RunningSessionMoment.WaitingResult;
                var playerValue = _currentTablePlayerCard.GetValue(IsStronger(playerSuit, enemySuit) ? 1: 0);
                var enemyValue = _currentTableEnemyCard.GetValue(IsStronger(enemySuit, playerSuit) ? 1: 0);

                if(playerValue == enemyValue)
                {
                    DrawTurnResult();
                }
                else
                {
                    VictoryTurnResult(playerValue > enemyValue);
                }
        }
        private void CleanTable()
        {
           Destroy(_currentTablePlayerCard.gameObject);
           Destroy(_currentTableEnemyCard.gameObject);
        }

        private void DrawTurnResult()
        {
            _steak += 1;
            GameEvents.UI.onWinnerUpdate?.Invoke(_currentTurn, null);
        }

        private void VictoryTurnResult(bool isPlayerWinner)
        {
            if(isPlayerWinner)
            {
                _playerScore += _steak;
                GameEvents.UI.onWinnerUpdate?.Invoke(_currentTurn, _playerBody.GetAvatarSprite());
            }
            else
            {
                _enemyScore += _steak;
                GameEvents.UI.onWinnerUpdate?.Invoke(_currentTurn, _enemyBody.GetAvatarSprite());
            }

            _steak = 1;
        }

        private void NextTurn()
        {
            _playerCardData = null;
            _enemyCardData = null;
            _currentTurn ++;
            Debug.Log(_currentTurn + " / " + _totalTurn);
            if(_currentTurn <= _totalTurn)
            {                
                _sessionMoment = RunningSessionMoment.WaitingPlayer;
                GameEvents.UI.onRoundUpdate?.Invoke(_currentTurn, _totalTurn);
            }
        }
        private void PlayerThrowCard(CardView _cardObject, CardDefinition data)
        {
            if(!(_sessionMoment == RunningSessionMoment.WaitingPlayer))
                return;

            _cardObject.transform.position = _tablePlayerCardPosition.transform.position;
            _cardObject.transform.SetParent(_tablePlayerCardPosition.transform);
            _playerCardData = data;
            _currentTablePlayerCard = _cardObject;
            _sessionMoment = RunningSessionMoment.WaitingEnemy;

        }

        private void EnemyThrowCard(CardView _cardObject, CardDefinition data)
        {
            if(!(_sessionMoment == RunningSessionMoment.WaitingEnemy))
                return;

            _cardObject.transform.position = _tableEnemyCardPosition.transform.position;
            _cardObject.transform.SetParent(_tableEnemyCardPosition.transform);
            _currentTableEnemyCard = _cardObject;
            _enemyCardData = data;
        }

        private bool IsStronger(Suit suitAttacker, Suit suitDefender)
        {
            return (suitAttacker == Suit.Rock && suitDefender == Suit.Scissors) ||
           (suitAttacker == Suit.Paper && suitDefender == Suit.Rock) ||
           (suitAttacker == Suit.Scissors && suitDefender == Suit.Paper);
        }
    }
}
