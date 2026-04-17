using Godot;
using System;



public partial class SlimeAndGo : Node
{
#region Main


	public void Test(IMovementEntity testEntity, float delta)
	{
		PhysicsShapeQueryParameters2D testquery = CreateQuery(testEntity);

		GD.Print($@"
		[DATA von QUERY]
	My Position is at : {testquery.Transform.Origin}
	My Velocity is : {testquery.Motion}
	My Shape is : {testquery.Shape}
		");

		Vector2 TestVelocity = CreateInitialVector(testquery, testEntity, delta);
		GD.Print($"TestVelocity = {TestVelocity}");
		ApplyVector(TestVelocity, testEntity);
	}
	public void Move(Vector2 inputVector, IMovementEntity callingEntity, float delta)
	{
		float speed = callingEntity.SpeedMulti; //get speed of Caller
		Vector2 motionRequest = callingEntity.Velocity; //get NORMALIZED! Input Vector - rename to InputVector please
		//Vector2 initialVector = CreateInitialVector(motionRequest, entity, delta); //get the initial Vector out of the Input and Speed
		//jump would be handeled by using the jumpforce of interface - how do i want to go over that?
		//should i work gravity in here? maybe no

		//Vector2 validVector = GetValidVector(initialVector);//get/make a valid Vector 
		//ApplyVector(validVector);//apply the valid Vector
		SetFlags();//set flags for states
	}
	
	private Vector2 GetValidVector(Vector2 initialVector)
	{
		//make a valid vector:
		//check for collisions and remove vector into collision surface
		//remaining vector along the surface 
		//check for collisions along remaining vector
		//return valid vector
		Vector2 validVector = Vector2.Zero;




		return validVector;
	}

	private void ApplyVector(Vector2 validVector, IMovementEntity callingEntity)
	{
		var node = callingEntity as Node2D;
		GD.Print($"before moving node.GlobalPosition is : {node.GlobalPosition}");
        GD.Print($"I move by : {validVector}");
       if (node is not null)
        {
            node.GlobalPosition += validVector;
			GD.Print($"after moving node.GlobalPosition is : {node.GlobalPosition}");
        }
	}

	private void SetFlags()
	{
		//Update flags
		//flags like sliding, airborne, jumping, on ground etc. 
	}

	private void ApplyFlags()
	{
		//get updated flags
		//apply to calling entity
	}
#endregion	
#region Helper

private Vector2 CreateInitialVector(PhysicsShapeQueryParameters2D query, IMovementEntity entity,  float delta)
	{
		//get input Vector NORMALIZED! speed and delta to create initialMotion
		Vector2 initialMotion = query.Motion * entity.SpeedMulti * delta;

		return initialMotion;
	}

private PhysicsShapeQueryParameters2D CreateQuery (IMovementEntity callingEntity)
	{
			PhysicsShapeQueryParameters2D originalQuery = new PhysicsShapeQueryParameters2D();
			originalQuery.CollideWithAreas = false; //if true, query will take Area2D into account
			originalQuery.CollideWithBodies = true; //if true, query will take PhysicsBody2D into account
			//originalQuery.CollisionMask = ?
			originalQuery.Margin = 0.0f;
			originalQuery.Motion = callingEntity.Velocity; // here check what velocity does!
			originalQuery.Shape = callingEntity.MyShape.Shape;
			originalQuery.Transform = callingEntity.MyShape.GlobalTransform;

			// TODO: create debug tests for query output (static, small motion, edge cases)
			// verify what PhysicsShapeQueryParameters2D actually represents in space

		return originalQuery;
	}
#endregion
}

