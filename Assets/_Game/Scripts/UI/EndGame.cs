using UnityEngine;
using TMPro;
using UnityEngine.UI;
using RockPaperPistol.UI.Elements;
using RockPaperPistol.Events;
using System.Collections;
namespace RockPaperPistol.UI
{ 
    public class EndGame : MonoBehaviour
    {
        [SerializeField] private GameObject _object;
        [SerializeField] private TextMeshProUGUI _winnerText;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private Image _playerImage;
        [SerializeField] private Image _enemyImage;
        [SerializeField] private GenericButton _nextPhase;
        [SerializeField] private TextMeshProUGUI _nextPhaseText;

        private bool _isWinner;
        private const string WINNER_TEXT = "Você Venceu";
        private const string LOOSE_TEXT = "Você Perdeu";

        void Start()
        {
            Initialize();
        }

        void OnEnable()
        {
            GameEvents.UI.OnEndGame += DefineEndGameText;
        }

        void OnDisable()
        {
            GameEvents.UI.OnEndGame -= DefineEndGameText;
        }

        private void Initialize()
        {
            _nextPhase.Initialize(NextPhase);
            _object.gameObject.SetActive(false);
        }

        private void NextPhase()
        {
            _object.gameObject.SetActive(false);

            if (!_isWinner)
            {
                Utils.Utils.LoadScene("MainMenu");
            }
            else
            {
                if(GameManager.Instance.CurrentLevel > 2)
                {
                    Utils.Utils.LoadScene("MainMenu");
                }
                else
                {
                    Utils.Utils.LoadScene("SampleScene");
                }
            }
        }

        public void DefineEndGameText(bool isWinner, int playerScore, int enemyScore, Sprite playerImage, Sprite enemyImage)
        {
            _object.gameObject.SetActive(true);
            _winnerText.text = isWinner ? WINNER_TEXT : LOOSE_TEXT;
            _scoreText.text = playerScore + " - " + enemyScore;
            _playerImage.sprite = playerImage;
            _enemyImage.sprite = enemyImage;
            _isWinner = isWinner;

            if (!isWinner)
            {
                _nextPhaseText.text = "Main Menu";
            }
            else
            {
                if(GameManager.Instance.CurrentLevel > 2)
                {
                    _nextPhaseText.text = "Main Menu";
                }
                else
                {
                    GameManager.Instance.SetLevel(GameManager.Instance.CurrentLevel + 1);
                    _nextPhaseText.text = "Next Game";
                }
            }
        }

    }
}