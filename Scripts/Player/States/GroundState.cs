using Godot;
using System;
using System.Transactions;

public partial class GroundState: IEntityState
{
    
    public void Exit(IEntity entity){}
    public void Enter(IEntity entity)
    {
        var vel = entity.Velocity;
        vel.Y = 0;
        entity.Velocity = vel;
    }
    public virtual IEntityState HandleInput(IEntity entity, double delta)
    {
         var velocity = entity.Velocity;
        if (!entity.IsOnGround)
        {
            return new JumpState();
        }
        
        //to ground the entity and stop it from falling.
        if (entity.IsOnGround)
        {
            velocity.Y = 0;
        }
        
            

        if (Input.IsActionPressed("mleft"))
        {
            velocity.X -= 1;
        }

        if (Input.IsActionPressed("mright"))
        {
            velocity.X += 1;
        }

        if (Input.IsActionJustPressed("jump"))
        {
            velocity.Y -= 1;
            return new JumpState();
        }
        entity.Velocity = velocity;
        return this;
    }
}