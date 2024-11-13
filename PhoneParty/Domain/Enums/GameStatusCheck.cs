namespace PhoneParty.Domain.Enums;

public enum GameStatusCheck
{
    Correct,
    LessThenMinimumAmountOfPlayers,
    MoreThenMaximumAmountOfPlayers,
    NoGameDefined,
    GameInProgress
}