using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;



/// <summary>
/// Eine class, die Bewegungs- und Collisionregeln managed und applied
/// Callable via SlimeAndMove.Go
/// </summary>
public partial class SlimeAndMove
{
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]

    struct SweepData
    {
        // '?' after the type means it can be null
        public Vector2? collisionPoint;
        public float safeMotionMargin;
        public Vector2? Normal;

        private string GetDebuggerDisplay()
        {
            return ToString();
        }
        //maybe add a Collider Reference


    }
    /// <summary>
    /// Checks for collisions and illegal motions into surfaces and modifies that motion if needed.
    /// </summary>
    /// <param name="proposedMotion">New unvalidated motion request </param>
    /// <param name="movementEntity">Object eines beweglichen Entity</param>
    /// <returns>A Vector2 that is allowed to be followed</returns>
    private Vector2 EnforceMotionLaws(Vector2 proposedMotion, PhysicsShapeQueryParameters2D query, IMovementEntity entity)
    {
        var node = entity as Node2D;
        if (node is null)
        {   
            GD.PrintErr($"{nameof(SlimeAndMove)} requires IMovementEntity to be a Node2D");
            return Vector2.Zero;
        }
        Vector2 startPosition = entity.MyPosition;
        //GD.Print("=== Start EnforceMotionLaws ===");
        //GD.Print("Entity Pos =", entity.MyPosition);
       // GD.Print("Entity Shape Pos  =", entity.MyShape.GlobalPosition);
        //GD.Print("Query Pos  =", query.Transform.Origin);
        PhysicsShapeQueryParameters2D sweepQuery = CreateQueryObject(entity, proposedMotion);
        SweepData Sweep = ShapeSweeper(sweepQuery, node);
        if (Sweep.Normal is null)
        {
            GD.Print("I Hit Nothing!");
            return proposedMotion;
        }
        float dot = proposedMotion.Dot((Vector2)Sweep.Normal);
        if (dot > 0)
        {
            return proposedMotion;
        }
        PhysicsShapeQueryParameters2D loopQuery = CreateQueryObject(entity, proposedMotion);
        
        Vector2 finalVector = LoopHelper(loopQuery, node);
        return finalVector;
    }

    private void KeepBoundaries()
    {
        // TODO: Implement KeepBoundaries to detect overlaps and return a small correction motion.
// Should check the entity's collision shape at its current position,
// average normals if multiple overlaps, and return a vector to nudge the player out.
        Vector2 pushLänge = new Vector2(3,3); //ersetze mit 1/3 Playerlength. IMovemententity hat noch keinen Zugang zu Playerlength or PlayerCollisionBox


    }
    private void MoveBy(Vector2 moveBy, IMovementEntity movementEntity)
    {
       var node = movementEntity as Node2D;
        GD.Print($"I move by : {moveBy}");
       if (node is not null)
        {
            node.GlobalPosition += moveBy;
        }
        else
        {
            GD.PrintErr($"{nameof(SlimeAndMove)} requires IMovementEntity to be a Node2D");
        }
    }

    private void UpdateCollisionFlags(Vector2 allowedMotion, IMovementEntity movementEntity)
    {
        
    }

    public void Go(IMovementEntity movementEntity, float delta)
    {
       const float speed = 200f;
    Vector2 input = Vector2.Zero;
    input.X = Input.GetActionStrength("mright") - Input.GetActionStrength("mleft");
    input.Y = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");

    if (input == Vector2.Zero)
        return;
    GD.Print($"Input X = {input.X}, Input Y = {input.Y}");
    Vector2 desiredMotion = input.Normalized() * speed * delta;
    PhysicsShapeQueryParameters2D query = CreateQueryObject(movementEntity, desiredMotion);
    Vector2 finalMotion = EnforceMotionLaws(desiredMotion, query, movementEntity);
    MoveBy(finalMotion, movementEntity); 
    }

#region Helperfunction
   

    public Vector2  LoopHelper (PhysicsShapeQueryParameters2D loopQuery, Node2D node)
    { 
        
        Vector2 startPosition = loopQuery.Transform.Origin;
        Vector2 originalMotion = loopQuery.Motion;
        Vector2 nextPosition = startPosition;
        Vector2 remainingMotion = originalMotion;
        GD.Print($"Before Loop starts position is at : {loopQuery.Transform}");
        float MinLength = 0.1f;
        int MaxIterations = 3;
        for (int i = 0; remainingMotion.LengthSquared()  > (MinLength*MinLength) && i < MaxIterations; i ++)
        {
            SweepData loopSweep = ShapeSweeper(loopQuery, node); 
            if (loopSweep.Normal is null)
            {
                nextPosition += remainingMotion;
                break;
            }
            Vector2 normal = (Vector2)loopSweep.Normal;
            float margin = loopSweep.safeMotionMargin;


            Vector2 safeTravelMotion = remainingMotion * margin;
            nextPosition += safeTravelMotion;
           
            Vector2 leftoverMotion = remainingMotion - safeTravelMotion;

            float dot = leftoverMotion.Dot(normal);
            
            Vector2 blockedMotion = dot * normal;

            remainingMotion = leftoverMotion - blockedMotion;
            

            
            loopQuery.Transform = new Transform2D(0, nextPosition);
            loopQuery.Motion = remainingMotion;

             
        }
        Vector2 finalMotion = nextPosition - startPosition;
        return finalMotion;
    }

    private SweepData ShapeSweeper (PhysicsShapeQueryParameters2D sweepQuery, Node2D node)
    {
        float epsilon = 1f;
        var spaceState2D = node.GetWorld2D().DirectSpaceState;
        Dictionary restingOverlapData = spaceState2D.GetRestInfo(sweepQuery);
       GD.Print($"sweepQuery Position is : {sweepQuery.Transform}");
        if (restingOverlapData is not null && restingOverlapData.Count > 0)//checking if already overlapping - early out
        {
            GD.Print("Already Colliding!");
            //!!ALREADY COLLIDING ON START. WHAT IS HAPPENING?
           SweepData wombo = new SweepData
            {
                collisionPoint = (Vector2)restingOverlapData["point"],
                safeMotionMargin = 0, //no safe motion since overlap detected
                Normal = (Vector2)restingOverlapData["normal"],
            };
            restingOverlapData.Clear();
            return wombo;
        }

        float[] motionCastData = spaceState2D.CastMotion(sweepQuery);
        Vector2 startPosition = sweepQuery.Transform.Origin;
        if (motionCastData[0] == 1)//no overlap detected in given motion - early out
        {
            return new SweepData
            {
                collisionPoint = null, //no collision along the path
                safeMotionMargin = 1,  //full motion is valid
                Normal = null, //no surface interaction -> no normal
            };
        }
        Vector2 endCollisionPoint = startPosition + (motionCastData[0] * sweepQuery.Motion) - (sweepQuery.Motion.Normalized() * epsilon);//is the CastMotion data normalized or not. AWARE!!
        PhysicsShapeQueryParameters2D restQuery = new();
        restQuery.Shape = sweepQuery.Shape;
        restQuery.Transform = new Transform2D(0, endCollisionPoint);
        restQuery.Motion = Vector2.Zero;
        restQuery.CollideWithAreas = sweepQuery.CollideWithAreas;
        restQuery.CollideWithBodies = sweepQuery.CollideWithBodies;
        restingOverlapData = spaceState2D.GetRestInfo(restQuery);
        
        if (restingOverlapData is null || restingOverlapData.Count == 0)
        {
            return new SweepData
            {
                collisionPoint = null, //no collision along the path
                safeMotionMargin = 1,  //full motion is valid
                Normal = null, //no surface interaction -> no normal
            };
        }
        
        return new SweepData
        {
            collisionPoint = (Vector2)restingOverlapData["point"],
            safeMotionMargin = motionCastData[0],
            Normal = (Vector2)restingOverlapData["normal"],
        };
    } 

    private PhysicsShapeQueryParameters2D CreateQueryObject(IMovementEntity entity, Vector2 motion)
    {
        PhysicsShapeQueryParameters2D query = new PhysicsShapeQueryParameters2D();
        query.Shape = entity.MyShape.Shape;
        query.Transform = new Transform2D(0, entity.MyShape.GlobalPosition); 
        GD.Print($"CreateQueryObject creates query at :{query.Transform} position");
        query.Motion = motion;
        //Areas usally used for non-solids, trigger events like damage zones, death planes, checkpoints etc. 
        query.CollideWithAreas = false;
        //Bodies usally used for Solids like walls, floors, enemies etc. 
        query.CollideWithBodies = true;
        return query;
    }  
#endregion 
}

