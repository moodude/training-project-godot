using Godot;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;


public partial class HookRadius : Area2D
{








// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		
	}


	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
		
	}
	private float GetShapeSize()
	{
		var nodeCollisionShape = (CollisionShape2D) GetNode("CollisionShape2D");

		var shapesize = (CircleShape2D) nodeCollisionShape.Shape;
	
		return shapesize.Radius;
	}

	
	
	
}
	
	
