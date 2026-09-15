using RockPaperPistol.Utils;
using UnityEngine;
using RockPaperPistol.UI.Elements;

namespace RockPaperPistol.UI
{
    public class WeaponSelectMenu : MonoBehaviour
    {
        [SerializeField] private GenericButton _currentSelectedButton;
        [SerializeField] private GenericButton _gunManButton;
        [SerializeField] private GenericButton _mummyButton;
        [SerializeField] private GenericButton _pirateButton;
        [SerializeField] private GenericButton _stoneStatuedButton;
        [SerializeField] private GenericButton _startButton;
        [SerializeField] private GenericButton _backMenuButton;

        [SerializeField] private PistolId _currentPistolSelect;

        private static string SCENE_MENU_NAME = "MainMenu";
        private static string SCENE_GAME_NAME = "SampleScene";


        void Start()
        {
            _startButton.SetInteractableButtonState(false);
            _gunManButton.Initialize(SelectedButton);
            _mummyButton.Initialize(SelectedButton);
            _pirateButton.Initialize(SelectedButton);
            _stoneStatuedButton.Initialize(SelectedButton);
            _backMenuButton.Initialize(BackMenu);
            _startButton.Initialize(StartGame);

        }

        private void BackMenu()
        {
            Utils.Utils.LoadScene(SCENE_MENU_NAME);        }

        private void StartGame()
        {
            GameManager.Instance.DefineHeroPistol(_currentPistolSelect);
            Utils.Utils.LoadScene(SCENE_GAME_NAME);
        }

        private void SelectedButton(int buttonIndex, GenericButton selectedButton)
        {
            if(_currentSelectedButton == selectedButton)
            {
                _currentSelectedButton = null;
            }
            else
            {
                _currentPistolSelect = (PistolId) buttonIndex;
                _currentSelectedButton = selectedButton;
            }
            _startButton.SetInteractableButtonState(_currentSelectedButton != null);

        }
    }
}