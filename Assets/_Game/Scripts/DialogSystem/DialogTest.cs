using System.Collections;
using RockPaperPistol.Data;
using UnityEngine;

namespace RockPaperPistol.DialogSystem
{
    public class DialogTest : MonoBehaviour
{
    [SerializeField] private DialogData _test;
    [SerializeField] private DialogSystem _system;
    private bool _isShowingDialogTest = false;
    public void Start()
    {
        StartCoroutine(ShowDialog());
        _system.CallbackFinishWriteDialogPart += NextDialog;
    }

    IEnumerator ShowDialog()
    {
        DialogPart currentDialog;
        currentDialog = _test.LoadNextSequence(0);
        var count = 0;
        while (currentDialog != null)
        {
            _isShowingDialogTest = true;
            _system.ShowDialogUI(currentDialog);
            count++;
            currentDialog = _test.LoadNextSequence(count);
            yield return new WaitUntil(() => !_isShowingDialogTest);
        }
    }

    private void NextDialog()
    {
        _isShowingDialogTest = false;
    }
}
}
