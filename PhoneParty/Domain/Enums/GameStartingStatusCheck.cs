namespace Domain.Enums;

public enum GameStartingStatusCheck
{
    Correct,
    LessThenMinimumAmountOfPlayers,
    MoreThenMaximumAmountOfPlayers,
    NoGameDefined,
    GameInProgress
}