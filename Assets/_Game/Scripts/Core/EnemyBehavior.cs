namespace RockPaperPistol.Core
{
    public enum EnemyBehavior
    {
        Defensive = 0,
        Aggressive = 1
    }

    public static class EnemyBehaviorText
    {
        public static string Label(EnemyBehavior behavior)
        {
            return behavior == EnemyBehavior.Aggressive ? "agressivo" : "defensivo";
        }
    }
}
