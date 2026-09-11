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

        [SerializeField] private PistolsEnum _currentPistolSelect;

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
            GameFlow.GoToMenu();
        }

        private void StartGame()
        {
            if (GameFlow.Current != null)
            {
                GameFlow.Current.SelectPistol(_currentPistolSelect);
            }

            GameEvents.UI.onSelectPistol?.Invoke((int)_currentPistolSelect, _currentSelectedButton);
            GameFlow.GoToBattle();
        }

        private void SelectedButton(int buttonIndex, GenericButton selectedButton)
        {
            if(_currentSelectedButton == selectedButton)
            {
                _currentSelectedButton = null;
            }
            else
            {
                _currentPistolSelect = (PistolsEnum) buttonIndex;
                _currentSelectedButton = selectedButton;
            }
            _startButton.SetInteractableButtonState(_currentSelectedButton != null);
        }
    }
}