using System.Collections.Generic;
using UnityEngine;

namespace RockPaperPistol.Data
{
    [CreateAssetMenu(fileName = "Dialog", menuName = "Rock Paper Pistol/Dialogo")]
    public class DialogData : ScriptableObject
    {
        public List<DialogPart> SequenceDialog;
        public InterruptionType InterruptionType;

        public DialogPart LoadNextSequence(int countSequence)
        {
            DialogPart dialog = null;
            if(countSequence < SequenceDialog.Count)
            {
                dialog = SequenceDialog[countSequence];
            }
             return dialog;
        }
    }
}