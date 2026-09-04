using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using RockPaperPistol.Data;

namespace RockPaperPistol.DialogSystem
{
    public class DialogSystem : MonoBehaviour
    {
        [SerializeField] private Image _characterDisplay;
        [SerializeField] private TMPro.TextMeshProUGUI dialogueSentence;
        public Action CallbackFinishWriteDialogPart;

        public void ShowDialogUI(DialogPart dialog)
        {
            StartCoroutine(WriteDialog(dialog));
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

            CallbackFinishWriteDialogPart?.Invoke();

        }
    }
}

