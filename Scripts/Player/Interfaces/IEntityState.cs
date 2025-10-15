using Godot;
using System;
/// <summary>
/// Defines the behavior of the state for an entity. 
/// Includes how it enters, exits and how it handles Input.
/// </summary>
public interface IEntityState
{
    /// <summary>
    /// Called when the state is exited. Use this to clean up/reset values.
    /// </summary>
    /// <param name="entity">The entity leaving this state.</param>
    void Exit(IEntity entity);

    /// <summary>
    /// Called when the state is entered. Use this to initialize/set up logic or values.
    /// </summary>
    /// <param name="entity">The entity entering this state.</param>
    void Enter(IEntity entity);
    
    /// <summary>
    /// Handles input and logic for this state and determines whether a state transition is needed or not.
    /// </summary>
    /// <param name="entity">The entity being updated or processed.</param>
    /// <param name="delta">The time elapsed since the last frame.</param>
    /// <returns>
    /// The next <see cref="IEntityState"/> to transition to or <c>null</c> to remain the in current state.
    /// </returns>
    IEntityState HandleInput(IEntity entity, double delta);
}