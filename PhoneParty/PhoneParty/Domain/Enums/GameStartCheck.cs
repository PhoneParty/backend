namespace PhoneParty.Domain.Enums;

public enum GameStartCheck
{
    Successful,
    LessThenMinimumAmountOfPlayers,
    MoreThenMaximumAmountOfPlayers,
    NoGameDefined,
    GameInProgress
}