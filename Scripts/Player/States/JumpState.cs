using Godot;
using System;

public partial class JumpState: IEntityState
{
	public void Exit(IEntity entity)
    {
		
    }
    public void Enter(IEntity entity)
	{
		entity.CurrentGravity = entity.BaseGravity;
    }
	public virtual IEntityState HandleInput(IEntity entity, double delta)
	{
		if (entity.IsOnGround)
		{
			return new GroundState();
		}

		
		var vel = entity.Velocity;
		vel.Y += entity.CurrentGravity * (float)delta;  
		entity.Velocity = vel;

		return this;
	}
} 
