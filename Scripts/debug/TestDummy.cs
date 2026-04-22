using Godot;
using System;

public partial class TestDummy : Node2D, IMovementEntity
{
	#region Interface Implementation
	public Vector2 Velocity { get; set; }
    public float SpeedMulti { get; set; }
    public float JumpForce { get; set; }
    public float BaseGravity { get; set; }
    public float CurrentGravity { get; set; }

    public Vector2 MyPosition => Position;
    public CollisionShape2D MyShape => GetNode<CollisionShape2D>("CharacterBody2D/CollisionShape2D"); //every access call would GETNode , better to cache instead. fine for testing here

    

    public bool IsOnGround { get; }
    public bool IsInAir { get; }
    public bool IsTouchingLeft { get; }
    public bool IsTouchingRight { get; }

    public void HandleMovement(Vector2 direction, double delta){}

    public void HandleJumping(double delta){}

    public void HandleGravity(double delta){}

	#endregion

	#region Test Values

	private SlimeAndGo tester;
	public override void _Ready()
	{
		tester = new SlimeAndGo();
		SpeedMulti = 100f;
		JumpForce = 20f;
		//GD.Print($"My position is : {MyPosition}");
		//GD.Print($"My velocity is : {(Vector2)Velocity}");
		//GD.Print($"My shape is : {MyShape.Shape}");

	}
    public override void _PhysicsProcess(double delta)
    {
		
		Velocity = Vector2.Right;
        tester.Test(this, (float)delta); //this is cleaner and faster then GetNode
		
	}


	#endregion
}
