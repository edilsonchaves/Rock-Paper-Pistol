using RockPaperPistol.Utils;
using UnityEngine;
using RockPaperPistol.UI.Elements;
using RockPaperPistol.Data;
using System.Collections.Generic;

namespace RockPaperPistol.UI
{
    public class WeaponSelectMenu : MonoBehaviour
    {
        [SerializeField] private GenericButton _currentSelectedButton;
        [SerializeField] private GenericButton _gunManButton;
        [SerializeField] private GenericButton _mummyButton;
        [SerializeField] private GenericButton _pirateButton;
        [SerializeField] private GenericButton _stoneStatuedButton;
        [SerializeField] private GenericButton _selectWeaponButton;
        [SerializeField] private GenericButton _startButton;
        [SerializeField] private GenericButton _backMenuButton;
        [SerializeField] private GameObject _weaponMenu;
        [SerializeField] private GameObject _battleVersusUI;

        [SerializeField] private PistolId _currentPistolSelect;

        [SerializeField] private List<CardPistolDefinition> _pistols;

        private static string SCENE_MENU_NAME = "MainMenu";
        private static string SCENE_GAME_NAME = "SampleScene";

        void Start()
        {
            _selectWeaponButton.SetInteractableButtonState(false);
            _gunManButton.Initialize(SelectedButton);
            _mummyButton.Initialize(SelectedButton);
            _pirateButton.Initialize(SelectedButton);
            _stoneStatuedButton.Initialize(SelectedButton);
            _backMenuButton.Initialize(BackMenu);
            _selectWeaponButton.Initialize(SelectWeapon);
            _startButton.Initialize(StartGame);
        }

        private void BackMenu()
        {
            Utils.Utils.LoadScene(SCENE_MENU_NAME);
        }

        private void SelectWeapon()
        {
            GameManager.Instance.DefineHeroPistol(_pistols[(int) _currentPistolSelect]);
            _weaponMenu.SetActive(false);
            _battleVersusUI.SetActive(true);
        }

        private void StartGame()
        {
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
            _selectWeaponButton.SetInteractableButtonState(_currentSelectedButton != null);
        }
    }
}