using Godot;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

public partial class Player : Node2D, IEntity
{
    #region Fields
    private StateManager stateManager;
    private IEntityState initalState => new GroundState();
    private List<RayCast2D> collidingRays;
    private RayCastManager rayCastManager;
    #endregion

    #region Interface IEntity Implementiation

    public Vector2 Velocity { get; set; }
    public int CurrentHealth { get; set; }

    // Expression-bodied properties are shorthand for simple get methods
    // They allow you to directly return a value based on a single expression.
    // This syntax is more concise than using the traditional 'get' method syntax.
    // Example: 'public bool IsOnGround => collidingRays.Contains(rayCastManager.Down);'
    public bool IsOnGround => collidingRays.Contains(rayCastManager.Down);
    public bool IsInAir => !IsOnGround;
    public bool IsOnWall => collidingRays.Contains(rayCastManager.Left) || collidingRays.Contains(rayCastManager.Right);
    public bool IsAlive => CurrentHealth > 0;
    public bool IsDead => CurrentHealth <= 0;
    
        
    

    #endregion


    #region Properties
    [Export]
    public float speed = 3f;
    [Export]
    public float gravity = 2f;
    [Export]
    public int MaxHealth { get; set; } = 30;
    SignalBus signalBus;
    


    #endregion


    #region LifeCycle
    public override void _Ready()
    {
        signalBus = GetNode<SignalBus>("/root/SignalBus");
        signalBus.Damage += TakeDamage;
        signalBus.Respawn += Respawn;
        rayCastManager = GetNode<RayCastManager>("RayCastManager");

        InitState();

        CurrentHealth = MaxHealth;
    }

    public override void _PhysicsProcess(double delta)
    {
        collidingRays = rayCastManager.GetDirectionRays();
        stateManager.UpdateState(delta);
        GD.Print($"Velocity: {Velocity}");
        Position += new Vector2(Velocity.X*speed, Velocity.Y * gravity) * (float)delta;
    
    }

    #endregion

    #region Public Methods
    public void TakeDamage(int Amount)
    {
        CurrentHealth -= Amount;
        if (CurrentHealth <= 0)
        {
            GD.Print("Dead!");
        }
    }
    public void Respawn()
    {
        CurrentHealth = MaxHealth;
        GD.Print("Respawn!");
        //Position zu Respawnpoint
    }

    #endregion

    #region Private Methods
    private void InitState()
    {
        stateManager = new StateManager(this);
        stateManager.InitializeState(initalState);
    }

    #endregion

}

