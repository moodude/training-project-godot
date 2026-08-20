using Godot;
using Godot.Collections;
using System;




//TODO: Do this order:

//Stress test speed (no new features)
//Add gravity
//Add friction
//Add slide toggle
//Then corners




public partial class SlimeAndGo : Node2D
{
	private DebugVector debugCollisionNormalVector;
	private DebugVector debugCollisionPointVector;
	private DebugVector debugFinalVector;
#region Struct
	    struct SweepData
    {
        // '?' after the type means it can be null
        public Vector2? collisionPoint;
        public float safeMotionMargin;
        public Vector2? Normal;

    }

	public struct DebugVector//maybe add a frame summary beforehand like : player wanted (vector), player got (vector), loop entered: bool 
	{
		public Vector2 Start;
   		public Vector2 End;
    	public Color Color;

    	public DebugVector(Vector2 start, Vector2 end, Color color)
    	{
        	Start = start;
        	End = end;
        	Color = color;
    	}

		//public Vector2 SweepStart;
		//public Vector2 SweepEnd;
		//public Vector2? collisionNormal;
		//public Vector2? collisionPoint;
		//public Vector2? slidingVector;
		//public Vector2 remainingMotion;
		
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
		GD.Print($"TestVelocity = {TestVelocity}");
		GD.Print($"A BEFORE GetValidVector: {node.GlobalPosition}");
		Vector2 TestVector = GetValidVector(TestVelocity, testEntity);
		GD.Print($"B AFTER GetValidVector: {node.GlobalPosition}");
		GD.Print($"C - TestVector: {TestVector}");
		ApplyVector(TestVector, testEntity);
		GD.Print($"D - After ApplyVector: {node.GlobalPosition}");
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
	
	private Vector2 GetValidVector(Vector2 initialVector, IMovementEntity entity)
	{
		var node = entity as Node2D;
		//nullcheck?

		PhysicsShapeQueryParameters2D initialQuery = CreateQuery(entity);
		GD.Print($"POSITION: {node.GlobalPosition}");
GD.Print($"QUERY MOTION: {initialQuery.Motion}");
GD.Print($"INITIAL VECTOR: {initialVector}");
		SweepData initialSweep = ShapeSweeper(initialQuery, node);
		//GD.Print($"initialSweep.Normal: {initialSweep.Normal}");
		if (initialSweep.Normal is null)
		{
			//GD.Print("initialSweep = No Collision in the way");
			//GD.Print($"validVector before collision: {initialVector}");
			return initialVector;
		}
		float dot = initialVector.Dot((Vector2)initialSweep.Normal);
		//GD.Print($"dot product is : {dot}");
		if (dot > 0)
		{
			//GD.Print("Moving Away from Collision Surface!");
			return initialVector;	
		}


		//make a valid vector:
		//check for collisions and remove vector into collision surface
		//remaining vector along the surface 
		//check for collisions along remaining vector
		//return valid vector
		//GD.Print($"initalQuery.Motion: {initialQuery.Motion}" + $"initialVector: {initialVector}");
		Vector2 validVector = loopSweep(initialQuery, node);
		//GD.Print($"valid vector is : {validVector}");

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
		debugFinalVector = new DebugVector(Vector2.Zero,validVector, Colors.Red);
		//GD.Print($"Final vector: {validVector}");
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

private PhysicsShapeQueryParameters2D CreateQuery (IMovementEntity callingEntity) //check if i still want to use that!!
																				  // is called every frame in this setup
	{
			PhysicsShapeQueryParameters2D originalQuery = new PhysicsShapeQueryParameters2D();
			originalQuery.CollideWithAreas = true; //if true, query will take Area2D into account
			originalQuery.CollideWithBodies = false; //if true, query will take PhysicsBody2D into account
			//originalQuery.CollisionMask = ?
			originalQuery.Margin = 0.0f;
			originalQuery.Motion = callingEntity.Velocity; // here check what velocity does!
			originalQuery.Shape = callingEntity.MyShape.Shape;
			originalQuery.Transform = callingEntity.MyShape.GlobalTransform;
			//GD.Print($"Shape:  {callingEntity.MyShape.GlobalTransform.Origin}");
			
			// verify what PhysicsShapeQueryParameters2D actually represents in space
			//static and small motion test = values of variables are fine. no unexpected behavior at early point 22.04

		return originalQuery;
	}

private SweepData ShapeSweeper(PhysicsShapeQueryParameters2D originalQuery, Node2D node)
	{
		GD.Print($"SWEEP MOTION: {originalQuery.Motion}");
		var spaceState2D = node.GetWorld2D().DirectSpaceState;
		PhysicsShapeQueryParameters2D sweeperQuery = new();
		///resting position overlap check
		Dictionary restResultAtStart = spaceState2D.GetRestInfo(originalQuery);
		if(restResultAtStart.Count > 0) //might throw null reference or crash if null <- test if nullcheck needed? 
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
	//		GD.Print("EARLY REST HIT");
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
		//GD.Print($"Motion Cast Ergebnis = {motionCast[0]}");
		var startPosition = originalQuery.Transform.Origin;
		//GD.Print($"startPosition : {startPosition}");
		var endPosition = startPosition + originalQuery.Motion;
		//GD.Print($"end position without collision :{endPosition}");
		if (motionCast[0] == 1f ) //100% of motion 
		{
			//GD.Print("Motion Cast detected no collision");
			return new SweepData
				{
					collisionPoint = null,
					safeMotionMargin = 1,
					Normal = null
				};
		}
		else
		{
		//castMotion from new() position, motion
		endPosition = startPosition + motionCast[0] * originalQuery.Motion;
		//GD.Print($"end position with collision :{endPosition}");
		Vector2 endCollisionPoint = startPosition + (motionCast[0] + 0.01f) * originalQuery.Motion;
		//GD.Print($"end Collision Point : {endCollisionPoint}");
		sweeperQuery.Shape = originalQuery.Shape;
        sweeperQuery.Transform = new Transform2D(0, endCollisionPoint);
        sweeperQuery.Motion = Vector2.Zero;
        sweeperQuery.CollideWithAreas = originalQuery.CollideWithAreas;
        sweeperQuery.CollideWithBodies = originalQuery.CollideWithBodies;
			//resting position overlap check at the end
		//	GD.Print("Before GetRestInfo");
		Dictionary restResultAtEnd = spaceState2D.GetRestInfo(sweeperQuery);
		//GD.Print("After GetRestInfo");
			if(restResultAtEnd.Count > 0)
			{

				return new SweepData
				{
					collisionPoint = (Vector2)restResultAtEnd["point"],	
					safeMotionMargin = motionCast[0],
					Normal = (Vector2)restResultAtEnd["normal"]
				};
			}
			else
			{
				return new SweepData
				{
					collisionPoint = null,
					safeMotionMargin = 1,
					Normal = null
				};
			}	
		}
		
	}

private Vector2 loopSweep (PhysicsShapeQueryParameters2D loopQuery, Node2D node)
{
	Vector2 startPosition = loopQuery.Transform.Origin;
    Vector2 originalMotion = loopQuery.Motion;
    Vector2 nextPosition = startPosition;
    Vector2 remainingMotion = originalMotion;
    //GD.Print($"Before Loop starts position is at : {loopQuery.Transform}");
    float MinLength = 0.01f;
    int MaxIterations = 5;
	float epsilon = 0.05f;
	
    for (int i = 0; remainingMotion.LengthSquared()  > (MinLength*MinLength) && i < MaxIterations; i ++)
    {
    	SweepData loopSweep = ShapeSweeper(loopQuery, node); 
        if 	(loopSweep.Normal is null)
        {
        	nextPosition += remainingMotion;
            break;
        }
		Vector2 normal = (Vector2)loopSweep.Normal;
		//GD.Print($"Hit:    {loopSweep.collisionPoint}");
		debugCollisionPointVector = new DebugVector((Vector2)loopSweep.collisionPoint, (Vector2)loopSweep.collisionPoint, Colors.Green);
		debugCollisionNormalVector = new DebugVector((Vector2)loopSweep.collisionPoint,  normal, Colors.Blue);
		//GD.Print($"Normal: {normal}");
/*
GOAL: Stable kinematic sweep + slide movement (2D)

CURRENT APPROACH:
- I use iterative shape sweeps (max iterations)
- Each iteration:
    1. Sweep along remaining motion
    2. If collision happens, get ONE collision normal
    3. Move to safe position (before impact)
    4. Remove velocity component into the normal (slide)
    5. Continue with remaining motion

WHAT THIS ALREADY SOLVES:
- Prevents tunneling via sweeps
- Allows basic sliding along walls
- Works for simple single-surface collisions

CURRENT PROBLEMS:
- High-speed motion can re-hit same surface (sticking/jitter)
- Corners behave unstable (multiple surfaces)
- Floating point precision causes repeated collisions
- Motion may not fully separate from surfaces

WHAT I NEED TO ADD (STABILITY IMPROVEMENTS):
1. EPSILON OFFSET
   - Always stop slightly before collision point
   - Prevents re-colliding due to exact contact

2. SAFE MOVEMENT FRACTION (TOI margin)
   - Only move up to just before impact
   - Avoids penetrating or re-triggering collision

3. PROPER POSITION ADVANCEMENT
   - nextPosition MUST be updated each iteration:
     nextPosition += safeTravelMotion

4. CONSISTENT MOTION REDUCTION
   - After collision, remaining motion = tangent (no normal component)

RESULT GOAL:
- Stable sliding at any speed
- No sticking on walls
- Better corner behavior
- Deterministic iterative convergence within max iterations
*/

        float margin = loopSweep.safeMotionMargin;
		
		Vector2 safeTravelMotion = remainingMotion * margin;
		//GD.Print("Entity Position plus safeMotionMargin: " + (loopQuery.Transform.Origin + safeTravelMotion));
		//GD.Print($"Margin: {margin}");
		Vector2 leftoverMotion = remainingMotion - safeTravelMotion;
		
		float dot = leftoverMotion.Dot(normal);
		Vector2 tangent = leftoverMotion;
		if (dot < 0) //move into the wall
		{
			Vector2 blockedMotion = dot * normal;
			tangent = leftoverMotion - blockedMotion;
		}
		
		remainingMotion = tangent;
		nextPosition += safeTravelMotion;
		//GD.Print("Next Position before epsilon adjustment: " + nextPosition);
		if(safeTravelMotion.LengthSquared() < epsilon*epsilon)
		{
			//(nextPosition += safeTravelMotion - new Vector2(epsilon, 0);		
			//GD.Print("Next Position after epsilon adjustment: " + nextPosition);
		} 
		
		//UPDATE FOR NEXT ITERATION
		loopQuery.Transform = new Transform2D(0, nextPosition);
       	loopQuery.Motion = remainingMotion; 
		   
    }
    Vector2 finalMotion = nextPosition - startPosition;
	//GD.Print($"Final Motion after loopSweep: {finalMotion}");
    return finalMotion;	
}

public DebugVector GetDebugVector()
{
	
	return debugFinalVector;
}

public DebugVector GetDebugCollisionPoint()
{
	return debugCollisionPointVector;
}

public DebugVector GetDebugCollisionNormal()
{
	return debugCollisionNormalVector;
}
#endregion
}

