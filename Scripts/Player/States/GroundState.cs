using Godot;
using System;


public partial class GroundState: IEntityState
{
    
    public void Exit(IMovementEntity entity){}
    public void Enter(IMovementEntity entity)
    {
        //GD.Print($"[GroundState] Enter: setting gravity=0, resetting vel.Y");

        entity.CurrentGravity = 0;
        var vel = entity.Velocity;
        vel.Y = entity.CurrentGravity;
        entity.Velocity = vel;
        
        //GD.Print($"[GroundState] After Enter: Velocity={entity.Velocity}");
    }
    public virtual IEntityState HandleInput(IMovementEntity entity, double delta)
    {
        if (!entity.IsOnGround)
        {
            return new JumpState();
        }

        var velocity = entity.Velocity;

        if (Input.IsActionJustPressed("jump"))
        {
            //GD.Print($"[GroundState] Jump pressed. IsOnGround={entity.IsOnGround}");
            //GD.Print($"[GroundState] Before jump: Velocity={entity.Velocity}");
            entity.HandleJumping(delta);
            //GD.Print($"[GroundState] After jump: Velocity={entity.Velocity}");
            return new JumpState();
        }
 
        velocity.X = 0;
        if (Input.IsActionPressed("mleft"))
        {
            var direction = Vector2.Left;
            entity.HandleMovement(direction, delta);

        }


        if (Input.IsActionPressed("mright"))
        {
            var direction = Vector2.Right;
            entity.HandleMovement(direction, delta);

        }
        
        return this;
    }
}