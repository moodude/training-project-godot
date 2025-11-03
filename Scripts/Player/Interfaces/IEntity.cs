using Godot;
using System;

/// <summary>
/// Represents a game entity that can move, take damage and interact with the world. 
/// It defindes the physical stats. The Data.
/// </summary>
public interface IEntity
{   
    /// <summary>
    /// Gets or sets the jumpforce of the entity.
    /// </summary>
    float JumpForce { get; set; }
    /// <summary>
    /// Gets or sets the base gravity of the entity.
    /// </summary>
    float BaseGravity { get; set; }
    /// <summary>
    /// Gets or sets the gravity value of the entity.
    /// </summary>
    float CurrentGravity { get; set; }
    /// <summary>
    /// Gets or sets the speed multiplier of the entity.
    /// </summary>
    float SpeedMulti { get; set; }
    /// <summary>
    /// Gets or sets the current velocity of the entity.
    /// </summary>
    Vector2 Velocity { get; set; }


    /// <summary>
    /// Gets whether the entity is on the ground or not.
    /// </summary>
    bool IsOnGround { get; }
    /// <summary>
    /// Gets whether the entity is in the air or not.
    /// </summary>
    bool IsInAir { get; }
    /// <summary>
    /// Gets whether the entity is detecting Left collision or not.
    /// </summary>
    bool IsTouchingLeft { get; }
    /// <summary>
    /// Gets whether the entity is detecting right collision or not.
    /// </summary>
    bool IsTouchingRight { get; }
   
}