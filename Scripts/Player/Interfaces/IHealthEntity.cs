using Godot;
using System;

public interface IHealthEntity
{
    /// <summary>
    /// Gets or sets the current health value of the entity.
    /// </summary>
    int CurrentHealth { get; set; }
    /// <summary>
    /// Gets whether the entity is alive.
    /// </summary>
    bool IsAlive { get; }
    /// <summary>
    /// Gets whether the entity is dead.
    /// </summary>
    bool IsDead { get; }

    /// <summary>
    /// Applies damage to <see cref="CurrentHealth"/> by given amount.
    /// </summary>
    /// <param name="amount">the amount of damage to apply</param>
    void TakeDamage(int amount);
    /// <summary>
    /// Restores the entity to its initial state. 
    /// </summary>
    void Respawn();
}