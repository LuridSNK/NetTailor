namespace NetTailor.Contracts;

/// <summary>
/// Represents a void type, since <see cref="System.Void"/> is not a valid return type in C#.
/// </summary>
public struct Empty : IEquatable<Empty>, IComparable<Empty>, IComparable
{
    private static readonly Empty _value = new();

    /// <summary>
    /// Default and only value of the <see cref="Empty"/> type.
    /// </summary>
    public static ref readonly Empty Value => ref _value;

    /// <summary>
    /// Task from a <see cref="Empty"/> type.
    /// </summary>
    public static Task<Empty> Task { get; } = System.Threading.Tasks.Task.FromResult(_value);

    public int CompareTo(Empty other) => 0;

    int IComparable.CompareTo(object? obj) => 0;

    public override int GetHashCode() => 0;

    public bool Equals(Empty other) => true;

    public override bool Equals(object? obj) => obj is Empty;

    public static bool operator ==(Empty first, Empty second) => true;

    public static bool operator !=(Empty first, Empty second) => false;

    public override string ToString() => nameof(Empty);
}