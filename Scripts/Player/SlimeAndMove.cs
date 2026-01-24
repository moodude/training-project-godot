using Godot;
using System;
using System.Linq;

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
    public Vector2 EnforceMotionLaws(Vector2 proposedMotion, IMovementEntity movementEntity)
    {
        var node = movementEntity as Node2D;
        if(node is not null)
        {
            var spaceState = node.GetWorld2D().DirectSpaceState;
            var shape = new CircleShape2D();
            //hardcoded magic number. mach abhänging von Interface bitte!
            shape.Radius = 1;
            PhysicsShapeQueryParameters2D query = new PhysicsShapeQueryParameters2D();
            query.Shape = shape;
            query.Transform = node.Transform; // start at player position
            query.Motion = proposedMotion;
            //Areas usally used for non-solids, trigger events like damage zones, death planes, checkpoints etc. 
            query.CollideWithAreas = false;
            //Bodies usally used for Solids like walls, floors, enemies etc. 
            query.CollideWithBodies = true;

            var result = spaceState.GetRestInfo(query);

            //early out if no collision
            if(result is null || result.Count == 0)
            {
                return proposedMotion;
            }
            Vector2 normal = (Vector2)result["normal"];
            float dot = proposedMotion.Dot(normal);
            
            if (dot > 0)
            {
                var illegalMotion = dot * normal;
                //ist noch nicht vorbereitet und noch nicht da
                //if(sliding is pressed) 
                //{
                   // Vector2 tangent = new Vector2(-normal.Y, normal.X);
                   // Vector2 slideMotion = tangent * proposedMotion.Dot(tangent);
                   // return slideMotion;
                //}

                Vector2 allowedMotion = proposedMotion - illegalMotion;

                return allowedMotion;
            }

            return proposedMotion;
        }
        else
        {
          GD.PrintErr("Node is not Node2D");
          return Vector2.Zero;
        }
    }

    public void ApplyMotion(Vector2 allowedMotion, IMovementEntity movementEntity)
    {
        
    }

    public void UpdatePositionandFlags(Vector2 allowedMotion, IMovementEntity movementEntity)
    {
        
    }

    public void Go(IMovementEntity movementEntity, float delta)
    {
        
    }
}

