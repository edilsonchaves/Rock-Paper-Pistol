namespace RockPaperPistol.Data
{
    public enum AudioPlayTrigger
    {
        None = 0,
        SceneEnter = 1,
        GameplayEvent = 2,
        PistolCard = 3,
        SuitCheck = 4
    }

    public enum AudioStopTrigger
    {
        None = 0,
        Duration = 1,
        GameplayEvent = 2,
        SceneExit = 3
    }
}
