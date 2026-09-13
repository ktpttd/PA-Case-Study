namespace DuetCats.Session
{
    public enum GamePhase
    {
        Preparing = 0,
        Ready = 1,
        Playing = 2,
        Ending = 3,
        Result = 4,
        ContentError = 5
    }

    public enum GameOutcome
    {
        None = 0,
        Win = 1,
        Lose = 2
    }
}
