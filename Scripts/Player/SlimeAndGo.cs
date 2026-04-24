using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;





public partial class SlimeAndGo : Node
{
#region Struct
	    struct SweepData
    {
        // '?' after the type means it can be null
        public Vector2? collisionPoint;
        public float safeMotionMargin;
        public Vector2? Normal;

    }
#endregion
#region Main


	public void Test(IMovementEntity testEntity, float delta)
	{
		Node2D node = testEntity as Node2D;
		PhysicsShapeQueryParameters2D testquery = CreateQuery(testEntity);

		//GD.Print($@"
		//[DATA von QUERY]
	//My Position is at : {testquery.Transform.Origin}
	//My Velocity is : {testquery.Motion}
	//My Shape is : {testquery.Shape}
	//	");

		Vector2 TestVelocity = CreateInitialVector(testquery, testEntity, delta);
		SweepData LOL = ShapeSweeper(testquery, node);
		//GD.Print($"TestVelocity = {TestVelocity}");
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
		//GD.Print($"before moving node.GlobalPosition is : {node.GlobalPosition}");
        //GD.Print($"I move by : {validVector}");
       if (node is not null)
        {
            node.GlobalPosition += validVector;
			//GD.Print($"after moving node.GlobalPosition is : {node.GlobalPosition}");
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
		Vector2 initialMotion = query.Motion.Normalized() * entity.SpeedMulti * delta;

		return initialMotion;
	}

private PhysicsShapeQueryParameters2D CreateQuery (IMovementEntity callingEntity)
	{
			PhysicsShapeQueryParameters2D originalQuery = new PhysicsShapeQueryParameters2D();
			originalQuery.CollideWithAreas = true; //if true, query will take Area2D into account
			originalQuery.CollideWithBodies = false; //if true, query will take PhysicsBody2D into account
			//originalQuery.CollisionMask = ?
			originalQuery.Margin = 0.0f;
			originalQuery.Motion = callingEntity.Velocity; // here check what velocity does!
			originalQuery.Shape = callingEntity.MyShape.Shape;
			originalQuery.Transform = callingEntity.MyShape.GlobalTransform;

			
			// verify what PhysicsShapeQueryParameters2D actually represents in space
			//static and small motion test = values of variables are fine. no unexpected behavior at early point 22.04

		return originalQuery;
	}

private SweepData ShapeSweeper(PhysicsShapeQueryParameters2D originalQuery, Node2D node)
	{
		var spaceState2D = node.GetWorld2D().DirectSpaceState;
		PhysicsShapeQueryParameters2D sweeperQuery = new();
		///resting position overlap check
		Dictionary restResultAtStart = spaceState2D.GetRestInfo(originalQuery);
		if(restResultAtStart.Count != 0) //might throw null reference or crash if null <- test if nullcheck needed? 
		{
	//		GD.Print($@"At resting is a collision:
	//	Collision Point is : {restResultAtStart["point"]}
	//	Querys Shape Data is : {restResultAtStart["shape"]}
	//	Surface Normal is : {restResultAtStart["normal"]}
	//	Collider ID is : {restResultAtStart["collider_id"]}
	//	 ");
	//		ulong colliderID = (ulong)restResultAtStart["collider_id"];
	//		var collider = InstanceFromId(colliderID);
	//		GD.Print(collider);

			return new SweepData
			{
				collisionPoint = (Vector2)restResultAtStart["point"],	
				safeMotionMargin = 0,
				Normal = (Vector2)restResultAtStart["normal"]
			};
		}
		///end resting position overlap check
		
		///castMotion check for given query.motion
		float[] motionCast = spaceState2D.CastMotion(originalQuery);
		GD.Print($"Motion Cast Ergebnis = {motionCast[0]}");
		var startPosition = originalQuery.Transform.Origin;
		GD.Print($"startPosition : {startPosition}");
		var endPosition = startPosition + originalQuery.Motion;
		GD.Print($"end position without collision :{endPosition}");
		if (Mathf.IsEqualApprox(motionCast[0],1f)) //100% of motion 
		{
			GD.Print("Motion Cast detected no collision");
			return new SweepData
			{
				collisionPoint = null,
				safeMotionMargin = 1,
				Normal = null
			};
		}
		
		//castMotion from new() position, motion
			GD.Print("Motion Cast detected a collision");
			endPosition = startPosition + motionCast[0] * originalQuery.Motion;
			GD.Print($"end position with collision :{endPosition}");
			Vector2 endCollisionPoint = startPosition + (motionCast[0] + float.Epsilon) * originalQuery.Motion;
			GD.Print($"end Collision Point : {endCollisionPoint}");
			sweeperQuery.Shape = originalQuery.Shape;
        	sweeperQuery.Transform = new Transform2D(0, endCollisionPoint);
        	sweeperQuery.Motion = Vector2.Zero;
        	sweeperQuery.CollideWithAreas = originalQuery.CollideWithAreas;
        	sweeperQuery.CollideWithBodies = originalQuery.CollideWithBodies;
			//resting position overlap check at the end
			Dictionary restResultAtEnd = spaceState2D.GetRestInfo(sweeperQuery);
			if(restResultAtEnd.Count != 0)
			{

				return new SweepData
				{
					collisionPoint = (Vector2)restResultAtEnd["point"],	
					safeMotionMargin = 0,
					Normal = (Vector2)restResultAtEnd["normal"]
				};
			}
			else
			{
				return new SweepData
				{
					collisionPoint = (Vector2)restResultAtEnd["point"],
					safeMotionMargin = motionCast[0],
					Normal = (Vector2)restResultAtEnd["normal"]
				};
			}
		
	}
#endregion
}

