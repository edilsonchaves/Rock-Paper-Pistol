using System;
using UnityEngine;
using UnityEngine.UI;

public class GenericButton : MonoBehaviour
{

    private Action _clickAction;
    [SerializeField] private Button _button;

    public void Initialize(Action buttonAction)
    {
        _clickAction = buttonAction;
    }

    public void ClickButton()
    {
        _clickAction?.Invoke();
    }

    public void SetInteractableButtonState(bool isInteractable)
    {
        _button.interactable = isInteractable;
    }
}
