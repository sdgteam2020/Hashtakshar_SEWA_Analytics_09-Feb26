using System.ComponentModel.DataAnnotations;

namespace HastaksharSewaAnalytics.Domain.Premitives.Entity;

public abstract class Entity<TId> : IEquatable<Entity<TId>>
    where TId : notnull
{
    protected Entity(TId id) => Id = id;
    [Key]
    public TId Id { get; private init; }

    public override bool Equals(object? obj)
        => obj is Entity<TId> other
           && GetType() == other.GetType()
           && EqualityComparer<TId>.Default.Equals(Id, other.Id);

    public bool Equals(Entity<TId>? other) => Equals((object?)other);

    public override int GetHashCode()
        => HashCode.Combine(GetType(), Id);

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
        => Equals(left, right);

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
        => !Equals(left, right);
}
