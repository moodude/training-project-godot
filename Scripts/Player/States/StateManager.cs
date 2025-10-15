using Godot;
using System;
/// <summary>
/// Manages and transtitons between the state of a given <see cref= "IEntity"/> 
/// </summary>
public partial class StateManager : Node
{
    #region Fields

    private IEntityState entityCurrentState;
    private IEntity entity;

    #endregion

    #region Properties
    /// <summary>
    /// Gets the entity's currently active state.
    /// </summary>
    public IEntityState CurrentState => entityCurrentState;

    #endregion

    #region Constructors
    public StateManager(IEntity entity)
    {
        this.entity = entity;
    }

    #endregion

    #region Lifecycle

    /// <summary>
    /// Called every frame by Godot. Updates the current state if one is set.
    /// </summary>
    /// <param name="delta">The time elapsed since the last frame</param>
    public override void _Process(double delta)
    {
        if (entityCurrentState is null)
        {
            GD.PrintErr("UpdateState is not runnig because current State is null");
            return;
        }
        UpdateState(delta);//Brauch ich das? Ruft das UpdateState nur zweimal auf?
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Updates current state each frame by calling <see cref="IEntityState.HandleInput"/>
    /// If a new state is returned, it transitions to that state. 
    /// </summary>
    /// <param name="delta">The time elapsed since the last frame</param>
    public void UpdateState(double delta)
    {
        if (entityCurrentState is null)
        {
            return;
        }
        var nextState = entityCurrentState.HandleInput(entity, delta);
        if (nextState is null)
        {
            GD.PrintErr("UpdateState got a Null State");
            return;
        }
        if (entityCurrentState != nextState)
        {
            SetState(nextState);
        }
    }

    public void InitializeState(IEntityState initialState)
    {
        entityCurrentState = initialState;
        entityCurrentState.Enter(entity);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Switches from the current state to a new state, calling <see cref="IEntityState.Exit"/>  and <see cref="IEntityState.Enter"/> as needed. 
    /// </summary>
    /// <param name="newState">The new state to transition to.</param>
    private void SetState(IEntityState newState)
    {
        if (newState is null)
        {
            GD.PrintErr("State is Null");
            return;
        }

        if (entityCurrentState == newState)
        {
            return;
        }

        if (entityCurrentState is not null)
        {
            entityCurrentState.Exit(entity);
        }

        entityCurrentState = newState;

        entityCurrentState.Enter(entity);

    }

    #endregion
}