using Godot;
using System;

public partial class Driver : Node2D, IMovementEntity
{
	private SlimeAndMove _mover;

	public Vector2 MyPosition => GlobalPosition;
    public CollisionShape2D MyShape => GetNode<CollisionShape2D>("../Driver/CharacterBody2D/CollisionShape2D"); 
public Vector2 Velocity { get; set; }
 public   float SpeedMulti { get; set; }
 public   float JumpForce { get; set; }
  public  float BaseGravity { get; set; }
  public  float CurrentGravity { get; set; }

  public  bool IsOnGround { get; }
   public bool IsInAir { get; }
  public  bool IsTouchingLeft { get; }
  public  bool IsTouchingRight { get; }

  public void HandleMovement(Vector2 direction, double delta) {}
	

  public void HandleJumping(double delta){}

   public void HandleGravity(double delta){}

    public override void _Ready()
    {
        _mover = new SlimeAndMove();
    }

    public override void _PhysicsProcess(double delta)
    {
        _mover.Go(GetNode("../Driver") as IMovementEntity, (float)delta);
    }
}
