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
	private DebugDraw debugDraw;
	public override void _Ready()
	{
		debugDraw = new DebugDraw();
		tester = new SlimeAndGo();
		AddChild(debugDraw);
		SpeedMulti = 9000f;
		JumpForce = 500f;
		BaseGravity = 60f;
		//GD.Print($"My position is : {MyPosition}");
		//GD.Print($"My velocity is : {(Vector2)Velocity}");
		//GD.Print($"My shape is : {MyShape.Shape}");

	}
    public override void _PhysicsProcess(double delta)
    {
		//GD.Print($"My global position is : {GlobalPosition}");
		Vector2 input = Input.GetVector("mleft", "mright", "jump", "ui_down");
		Vector2 grav = Vector2.Down * BaseGravity * (float)delta;


		Velocity = input.Normalized();
        tester.Test(this, (float)delta); //this is cleaner and faster then GetNode
		//Velocity = grav;
		//tester.Test(this, (float)delta);
		var debugFinalVector = tester.GetDebugVector();
		debugDraw.AddLine(debugFinalVector.Start, debugFinalVector.End*100f, Colors.Red);
		var debugCollisionPointVector = tester.GetDebugCollisionPoint();
		debugDraw.AddLine(debugCollisionPointVector.End - GlobalPosition + new Vector2(-10f, 0), debugCollisionPointVector.End - GlobalPosition, debugCollisionPointVector.Color);
		var debugCollisionNormalVector = tester.GetDebugCollisionNormal();
		debugDraw.AddLine(Vector2.Zero, debugCollisionNormalVector.End * 50f, Colors.Blue);
		
	}


	#endregion
}
