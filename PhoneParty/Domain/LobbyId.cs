namespace PhoneParty.Domain;

public readonly struct LobbyId : IEquatable<LobbyId>
{
    private readonly string _value;

    private static readonly char[] AllowedCharacters = "ABCDEF0123456789".ToCharArray();
    private const int Length = 4;

    public LobbyId(string value)
    {
        if (value.Length != Length || !IsValid(value))
            throw new ArgumentException(
                $"Value must be a string of length {Length} containing only characters A-F and 0-9.");

        _value = value.ToUpper();
    }

    private static bool IsValid(string value) => value.All(c => AllowedCharacters.Contains(c)) && value.Length == Length;

    public override bool Equals(object? obj) => obj is LobbyId other && Equals(other);

    public bool Equals(LobbyId other) => _value == other._value;

    public override int GetHashCode() => _value.GetHashCode();

    public override string ToString() => _value;

    public static bool operator ==(LobbyId left, LobbyId right) => left.Equals(right);

    public static bool operator !=(LobbyId left, LobbyId right) => !(left == right);
}