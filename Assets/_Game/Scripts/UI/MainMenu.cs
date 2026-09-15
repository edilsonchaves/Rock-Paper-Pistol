using UnityEngine;
using System.Collections;
using RockPaperPistol.UI.Elements;
using UnityEngine.UI;

namespace RockPaperPistol.UI
    {
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private GenericButton _startButton;
        [SerializeField] private GenericButton _optionsButton;
        [SerializeField] private GenericButton _creditsButton;
        [SerializeField] private GenericButton _exitButton;
        [SerializeField] private Animator _animatorMenuUI;

        //Mudança de Rodolfo
        [SerializeField] private GameObject _returnButton;
        [SerializeField] private GameObject _mainMenuBox;
        [SerializeField] private GameObject _optionsMenuBox;
        [SerializeField] private GameObject _creditsMenuBox;
        [SerializeField] private GameObject _tutorialMenuBox;
        [SerializeField] private GameObject _tutorialButton;
        [SerializeField] private GameObject _backButton;
        [SerializeField] private GameObject _fowardButton;
        [SerializeField] private Sprite[] _tutorialPages;

        [SerializeField] private Image _tutorial;
        
        [SerializeField] private int _tutorialIndex = 0;

        //Acaba aqui as variaveis de mudan�a do Rodolfo

        private const string START_GAME_ANIMATION_TRIGGER = "StartGame";
        private const string SELECT_WEAPON_SCENE = "WeaponScene";

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _startButton.Initialize(StartButtonAction);
            _optionsButton.Initialize(OptionsButtonAction);
            _creditsButton.Initialize(CreditsButtonAction);
            _exitButton.Initialize(ExitButtonAction);
            ShowCurrentPage();
        }

        private void StartButtonAction()
        {
            StartCoroutine(StartButtonClick());
        }

        IEnumerator StartButtonClick()
        {
            _mainMenuBox.SetActive(false);
            SetButtonInteractable(false);
            _animatorMenuUI.SetTrigger(START_GAME_ANIMATION_TRIGGER);
            yield return null;

            while (_animatorMenuUI.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            {
                yield return null;
            }
            Utils.Utils.LoadScene(SELECT_WEAPON_SCENE);
        }

        private void OptionsButtonAction()
        {
            _mainMenuBox.SetActive(false);
            _optionsMenuBox.SetActive(true);
        }

        private void CreditsButtonAction()
        {
            _mainMenuBox.SetActive(false);
            _creditsMenuBox.SetActive(true);
        }

        public void ReturnButtonAction()
        {
            _tutorialMenuBox.SetActive(false);
            _optionsMenuBox.SetActive(false);
            _creditsMenuBox.SetActive(false);
            _mainMenuBox.SetActive(true);
        }

        public void TutorialButtonAction()
        {
            _optionsMenuBox.SetActive(false);
            _tutorial.sprite = _tutorialPages[0];
            _tutorialIndex = 0;
            _tutorialMenuBox.SetActive(true);
        }

        public void ShowNext()
        {
            if (_tutorialIndex < _tutorialPages.Length - 1)
            {
                _tutorialIndex++;
                ShowCurrentPage();
            }
        }

        public void ShowPrevious()
        {
            if (_tutorialIndex > 0)
            {
                _tutorialIndex--;
                ShowCurrentPage();
            }
        }

        private void ShowCurrentPage()
        {
            _tutorial.sprite = _tutorialPages[_tutorialIndex];
        }

        private void ExitButtonAction()
        {
            Debug.Log("Exit");
        }



        private void SetButtonInteractable(bool newValue)
        {
            _startButton.SetInteractableButtonState(newValue);
            _optionsButton.SetInteractableButtonState(newValue);
            _creditsButton.SetInteractableButtonState(newValue);
            _exitButton.SetInteractableButtonState(newValue);
        }
    }
}