using Godot;
using System;


public partial class SlimeAndMove
{
    
    public Vector2 EnforceMotionLaws(Vector2 proposedMotion, IMovementEntity movementEntity)
    {
        var node = movementEntity as Node2D;
        if(node is not null)
        {
            var spaceState = node.GetWorld2D().DirectSpaceState;
        }
        else
        {
          GD.Print
        }
		return allowedMotion;
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

