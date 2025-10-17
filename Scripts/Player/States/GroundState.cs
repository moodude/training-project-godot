using Godot;
using System;


public partial class GroundState: IEntityState
{
    
    public void Exit(IEntity entity){}
    public void Enter(IEntity entity)
    {
        entity.CurrentGravity = 0;
        var vel = entity.Velocity;
        vel.Y = entity.CurrentGravity;
        entity.Velocity = vel;
        GD.Print($"entity.Velocity is {entity.Velocity}");
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
            GD.Print($"JumpForce: {entity.JumpForce}");  // should be 6000
        GD.Print($"SpeedMulti: {entity.SpeedMulti}");  // should be 150
        GD.Print($"Velocity before setting jump: {entity.Velocity}");
            velocity.Y -= 1* entity.JumpForce;
         GD.Print($"Velocity after setting jump: {velocity}");
            entity.Velocity = velocity;
            return new JumpState();
        }
 
        velocity.X = 0;
        if (Input.IsActionPressed("mleft"))
        {
            velocity.X -= 1;

        }
       

        if (Input.IsActionPressed("mright"))
        {
            velocity.X += 1;

        }
        entity.Velocity = velocity * entity.SpeedMulti;
        return this;
    }
}