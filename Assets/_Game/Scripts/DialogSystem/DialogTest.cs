using System.Collections;
using RockPaperPistol.Data;
using RockPaperPistol.Utils;
using UnityEngine;

namespace RockPaperPistol.DialogSystem
{
    public class DialogTest : MonoBehaviour
{
    [SerializeField] private DialogData _test;
    private bool _isShowingDialogTest = false;

    public void Start()
    {
        StartCoroutine(ShowDialog());
    }

    void OnEnable()
    {
        GameEvents.Dialog.CallbackFinishWriteDialogPart += NextDialog;
    }

    void OnDisable()
    {
        GameEvents.Dialog.CallbackFinishWriteDialogPart -= NextDialog;
    }

    IEnumerator ShowDialog()
    {
        DialogPart currentDialog;
        currentDialog = _test.LoadNextSequence(0);
        var count = 0;
        while (currentDialog != null)
        {
            _isShowingDialogTest = true;
            GameEvents.Dialog.ShowDialog?.Invoke(currentDialog);
            count++;
            currentDialog = _test.LoadNextSequence(count);
            yield return new WaitUntil(() => !_isShowingDialogTest);
        }
        yield return new WaitForSeconds(2f);

        GameEvents.Dialog.CloseDialog?.Invoke();
    }

    private void NextDialog()
    {
        _isShowingDialogTest = false;
    }
}
}
