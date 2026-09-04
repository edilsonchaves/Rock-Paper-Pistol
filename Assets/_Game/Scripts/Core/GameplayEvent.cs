using System;

namespace RockPaperPistol.Core
{
    public enum GameplayEvent
    {
        CardSelected,
        EnemyCardSelected,
        CardsRevealed,
        SuitChecked,
        ValueChecked,
        PlayerWin,
        PlayerLose,
        Draw,
        CardDiscarded,
        TurnStart,
        TurnEnd,
        GameWin,
        GameLose
    }

    public static class GameplayEventBus
    {
        public static event Action<GameplayEvent> Raised;

        public static void Raise(GameplayEvent gameplayEvent)
        {
            Raised?.Invoke(gameplayEvent);
        }
    }
}
