using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using RockPaperPistol.Data;
using RockPaperPistol.Events;

namespace RockPaperPistol.DialogSystem
{
    public class DialogSystem : MonoBehaviour
    {
        [SerializeField] private GameObject _dialogObject;
        [SerializeField] private Image _characterDisplay;
        [SerializeField] private TMPro.TextMeshProUGUI dialogueSentence;

        void OnEnable()
        {
            GameEvents.Dialog.ShowDialog += ShowDialogUI;
            GameEvents.Dialog.CloseDialog += CloseDialog;
        }

        void OnDisable()
        {
            GameEvents.Dialog.ShowDialog -= ShowDialogUI;
            GameEvents.Dialog.CloseDialog -= CloseDialog;
        }

        public void ShowDialogUI(DialogPart dialog)
        {
            _dialogObject.SetActive(true);
            StartCoroutine(WriteDialog(dialog));
        }

        public void CloseDialog()
        {
            _dialogObject.SetActive(false);
        }

        IEnumerator WriteDialog(DialogPart dialog)
        {
            int count = 0;

            if (dialog.IsNeedClearDialog)
            {
                yield return new WaitForSeconds(0.5f);
                dialogueSentence.text = "";
            }

            while (count < dialog.Sentence.Length)
            {
                dialogueSentence.text += dialog.Sentence[count];
                yield return new WaitForSeconds(dialog.CharacterDisplayDelay);
                count++;
            }
            GameEvents.Dialog.CallbackFinishWriteDialogPart?.Invoke();
        }
    }
}

