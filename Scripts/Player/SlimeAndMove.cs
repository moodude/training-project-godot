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
        var result = QueryHelper(proposedMotion, node);
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
            var result2 = QueryHelper(remainingMotion, node);
            if (result2 is null || result2.Count == 0)
            {
                break;
            }
            Vector2 normal2 = (Vector2)result2["normal"];
            float dot2 = remainingMotion.Dot(normal2);

            if (dot2 > 0)
            {
                break;
            }

            var illegalMotion2 = dot2 * normal2;
            Vector2 tangent = new Vector2(-normal2.Y, normal2.X);
            Vector2 motionAlongTangent = (remainingMotion.Dot(tangent) * tangent) - illegalMotion2;
            remainingMotion = motionAlongTangent;
        }
        return remainingMotion;
        



        
        
    }
        //Vector2 tangent = new Vector2(-normal.Y, normal.X);
        //Vector2 slideMotion = tangent * proposedMotion.Dot(tangent); 
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
        
    }

#region Helperfunction
    public Dictionary QueryHelper(Vector2 newMotion, Node2D node)
    {
            var spaceState = node.GetWorld2D().DirectSpaceState;
            var shape = new CircleShape2D();
            //hardcoded magic number. mach abhänging von Interface oder Entity bitte!
            shape.Radius = 1;
            PhysicsShapeQueryParameters2D query = new PhysicsShapeQueryParameters2D();
            query.Shape = shape;
            query.Transform = node.Transform; // start at player position
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

