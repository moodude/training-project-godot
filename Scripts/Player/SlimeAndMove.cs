using Godot;
using Godot.Collections;
using System;


/// <summary>
/// Eine class, die Bewegungs- und Collisionregeln managed und applied
/// Callable via SlimeAndMove.Go
/// </summary>
public partial class SlimeAndMove
{
    /// <summary>
    /// Checks for collisions and illegal motions into surfaces and modifies that motion if needed.
    /// </summary>
    /// <param name="proposedMotion">New unvalidated motion request </param>
    /// <param name="movementEntity">Object eines beweglichen Entity</param>
    /// <returns>A Vector2 that is allowed to be followed</returns>
    private Vector2 EnforceMotionLaws(Vector2 proposedMotion, IMovementEntity movementEntity)
    {

        // TODO next session:
// - Refine sliding along tangents for corners / multiple collisions
// - Clean up variable names for clarity (normal2, dot2, motionAlongTangent, etc.)
// - Verify loop early-exit logic (break vs return) matches intended behavior
// - Test edge cases: tiny remaining motion, multiple walls, partially inside surfaces
// - Consider if QueryHelper should allow a “virtual position” for remainingMotion checks
        var node = movementEntity as Node2D;
        if(node is null)
        {   
            GD.PrintErr($"{nameof(SlimeAndMove)} requires IMovementEntity to be a Node2D");
            return Vector2.Zero;
        
        }
        var result = QueryHelper(proposedMotion, node, movementEntity);
         //early out if no collision
        if(result is null || result.Count == 0)
        {
            return proposedMotion;
        }

        Vector2 normal = (Vector2)result["normal"];
        float dot = proposedMotion.Dot(normal);     
        if (dot > 0)
        {
            return proposedMotion;
        }

        var illegalMotion = dot * normal;
        Vector2 allowedMotion = proposedMotion - illegalMotion;

        Vector2 remainingMotion = allowedMotion;
        int maxIterations = 2;
        float minLength = 0.01f;
        for (int i = 0; i < maxIterations && remainingMotion.Length() > minLength; i++)
        {
            GD.Print($"Leftover motion:{remainingMotion.Length()}");
            GD.Print($"Iteration:{i}");
            var loopResult = QueryHelper(remainingMotion, node, movementEntity);
            if (loopResult is null || loopResult.Count == 0)
            {
                break;
            }
            Vector2 loopNormal = (Vector2)loopResult["normal"];
            float loopDot = remainingMotion.Dot(loopNormal);

            if (loopDot > 0)
            {
                break;
            }

            remainingMotion -= remainingMotion.Dot(loopNormal)* loopNormal;
        }
        return remainingMotion;   
    }

    private void KeepBoundaries()
    {
        Vector2 pushLänge = new Vector2(3,3); //ersetze mit 1/3 Playerlength. IMovemententity hat noch keinen Zugang zu Playerlength or PlayerCollisionBox


    }
    private void MoveBy(Vector2 moveBy, IMovementEntity movementEntity)
    {
       var node = movementEntity as Node2D;
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

    Vector2 desiredMotion = input.Normalized() * speed * delta;

    Vector2 finalMotion = EnforceMotionLaws(desiredMotion, movementEntity);
    MoveBy(finalMotion, movementEntity); 
    }

#region Helperfunction
    public Dictionary QueryHelper(Vector2 newMotion, Node2D node, IMovementEntity entity)
    {
            var spaceState = node.GetWorld2D().DirectSpaceState;
            var OriginalShape = entity.MyShape.Shape;
            var shape = new CircleShape2D();
            shape.Radius = (OriginalShape as CircleShape2D).Radius * entity.MyShape.Scale.X;
            //hardcoded magic number. mach abhänging von Interface oder Entity bitte!
            PhysicsShapeQueryParameters2D query = new PhysicsShapeQueryParameters2D();
            query.Shape = shape;
            query.Transform = node.Transform * entity.MyShape.Transform; // start at player position
            query.Motion = newMotion;
            //Areas usally used for non-solids, trigger events like damage zones, death planes, checkpoints etc. 
            query.CollideWithAreas = false;
            //Bodies usally used for Solids like walls, floors, enemies etc. 
            query.CollideWithBodies = true;

            var result = spaceState.GetRestInfo(query); 

            return result;
    }

    public void LoopHelper (Vector2 motionInQuestion, Dictionary results)
    {
      //maybe put the loop here. maybe not lol
    }
#endregion 
}

