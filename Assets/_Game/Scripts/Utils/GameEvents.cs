using System;
using RockPaperPistol.Data;
namespace RockPaperPistol.Utils
{
    public static class GameEvents
    {
        public static class Dialog
        {
            public static Action<DialogPart> ShowDialog;
            public static Action CallbackFinishWriteDialogPart;
            public static Action CloseDialog;
        }
    }

}