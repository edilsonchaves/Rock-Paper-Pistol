using System;
using RockPaperPistol.Data;
using RockPaperPistol.UI.Elements;
using UnityEngine;

namespace RockPaperPistol.Events
{
    public static class GameEvents
    {
        public static class Audio
        {
            
        }
        
        public static class Dialog
        {
            public static Action<DialogPart> ShowDialog;
            public static Action CallbackFinishWriteDialogPart;
            public static Action CloseDialog;
        }

        public static class UI
        {
            public static Action<int, GenericButton> onSelectPistol;
            public static Action<int, int> onRoundUpdate;
            public static Action<int, Sprite> onWinnerUpdate;

        }
    }

}
