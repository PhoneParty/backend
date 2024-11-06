namespace PhoneParty.Domain.Interfaces;

public interface IGame
{
    public void HandleAction(IAction action);
    public bool IsFinished { get; set; }
    public event Action<IEnumerable<IDifference>> GameStateChanged;
}