using Godot;
using System;


public partial class DirectionCheck : RayCast2D
{

	private SignalBus customsignal;
	private Vector2 direction;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		customsignal = GetNode<SignalBus>("/root/SignalBus");
		direction = Vector2.Zero;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (IsColliding())
		{
			direction = GetCollisionNormal();
			customsignal.EmitSignal(nameof(SignalBus.RayCastDirection), direction);
			
		}

	}




}
	






