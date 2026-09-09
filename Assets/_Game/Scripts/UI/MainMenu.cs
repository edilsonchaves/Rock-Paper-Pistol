using UnityEngine;
using System;
using System.Collections;
public class MainMenu : MonoBehaviour
{
    [SerializeField] private GenericButton _startButton;
    [SerializeField] private GenericButton _optionsButton;
    [SerializeField] private GenericButton _creditsButton;
    [SerializeField] private GenericButton _exitButton;
    [SerializeField] private Animator _animatorMenuUI;

    private const string START_GAME_ANIMATION_TRIGGER = "StartGame";
    private const string SELECT_WEAPON_SCENE = "WeaponScene";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _startButton.Initialize(StartButtonAction);
        _optionsButton.Initialize(OptionsButtonAction);
        _creditsButton.Initialize(CreditsButtonAction);
        _exitButton.Initialize(ExitButtonAction);
    }

    private void StartButtonAction()
    {
        StartCoroutine(StartButtonClick());
    }

    IEnumerator StartButtonClick()
    {
        SetButtonInteractable(false);
        _animatorMenuUI.SetTrigger(START_GAME_ANIMATION_TRIGGER);
        yield return null;

        while (_animatorMenuUI.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }
        Utils.LoadScene(SELECT_WEAPON_SCENE);
    }

    private void OptionsButtonAction()
    {
        Debug.Log("Option");
    }

    private void CreditsButtonAction()
    {
        Debug.Log("Credits");
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
