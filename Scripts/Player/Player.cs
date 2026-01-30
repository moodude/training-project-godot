using Godot;
using System;
using System.Collections.Generic;




public partial class Player : Node2D, IMovementEntity
{
    #region Fields
    private StateManager stateManager;
    private IEntityState initalState => new GroundState(); //überprüfe ob das notwendig ist später
    private List<RayCast2D> collidingRays;
    private RayCastManager rayCastManager;

    #endregion

    #region Interface IEntity Implementiation

    public Vector2 Velocity { get; set; }
    public int CurrentHealth { get; set; }

    public float SpeedMulti { get; set; } = 150;

    public float JumpForce { get; set; } = 300;

    public float BaseGravity { get; set; } = 200;
    public float CurrentGravity { get; set; }

    public Vector2 MyPosition => GlobalPosition;
    public CollisionShape2D MyShape => GetNode<CollisionShape2D>("CollisionBox"); 

    // Expression-bodied properties are shorthand for simple get methods
    // They allow you to directly return a value based on a single expression.
    // This syntax is more concise than using the traditional 'get' method syntax.
    // Example: 'public bool IsOnGround => collidingRays.Contains(rayCastManager.Down);'
    public bool IsOnGround => collidingRays.Contains(rayCastManager.Down);
    public bool IsInAir => !IsOnGround;
    public bool IsTouchingLeft => collidingRays.Contains(rayCastManager.Left);
    public bool IsTouchingRight => collidingRays.Contains(rayCastManager.Right);
    public bool IsAlive => CurrentHealth > 0;
    public bool IsDead => CurrentHealth <= 0;
    
        
    

    #endregion


    #region Properties
    
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
        Position += Velocity * (float)delta;
        collidingRays = rayCastManager.GetDirectionRays();
        
        //GD.Print($"[Physics] Pos={Position}, Vel={Velocity}, IsOnGround={IsOnGround}");
        stateManager.UpdateState(delta);
        //GD.Print($"Velocity: {Velocity}");
        
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
    public void HandleMovement(Vector2 direction, double delta)
    {
        var vel = Velocity;
        GD.Print($"{Velocity}");
        // Possible: using Expression-bodied property "CanMoveHorizontaly" to simplify 
        // Warnung! : Airborne state kann immernoch zu tunneling führen. Andere Lösung oder Airborne specific vorgehen.
        if (!CanMoveHorizontaly(direction) && IsOnGround)
        {
             vel.X = 0;
        }
        else
        {
            vel.X = direction.X * SpeedMulti;
        }

        if (IsOnGround && direction == Vector2.Zero)
        {
            vel.X = 0;
        } 

        Velocity = vel;
    }

    public void HandleJumping(double delta)
    {
        var vel = Velocity;
        vel.Y -= JumpForce;
        Velocity = vel;
    }
    public void HandleGravity(double delta)
    {
        var vel = Velocity;
        vel.Y += CurrentGravity * (float)delta;
        Velocity = vel;
    }
    #endregion

    #region Private Methods
    private void InitState()
    {
        stateManager = new StateManager(this);
        stateManager.InitializeState(initalState);
    }

    private bool CanMoveHorizontaly(Vector2 dir)
    {
        if ((dir == Vector2.Left && !IsTouchingLeft) || (dir == Vector2.Right && !IsTouchingRight))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion
}

