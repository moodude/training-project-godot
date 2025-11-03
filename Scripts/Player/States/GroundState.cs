using Godot;
using System;


public partial class GroundState: IEntityState
{
    
    public void Exit(IEntity entity){}
    public void Enter(IEntity entity)
    {
        //GD.Print($"[GroundState] Enter: setting gravity=0, resetting vel.Y");

        entity.CurrentGravity = 0;
        var vel = entity.Velocity;
        vel.Y = entity.CurrentGravity;
        entity.Velocity = vel;
        
        //GD.Print($"[GroundState] After Enter: Velocity={entity.Velocity}");
    }
    public virtual IEntityState HandleInput(IEntity entity, double delta)
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
            velocity.Y -= 1* entity.JumpForce;
            entity.Velocity = velocity;
            //GD.Print($"[GroundState] After jump: Velocity={entity.Velocity}");
            return new JumpState();
        }
 
        velocity.X = 0;
        if (Input.IsActionPressed("mleft") && (entity.IsTouchingLeft == false))
        {
            velocity.X -= 1;

        }


        if (Input.IsActionPressed("mright") && (entity.IsTouchingRight == false))
        {
            velocity.X += 1;

        }
        velocity.X *= entity.SpeedMulti;
        entity.Velocity = velocity;
        return this;
    }
}