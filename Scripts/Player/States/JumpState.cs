using Godot;
using System;

public partial class JumpState: IEntityState
{
	public void Exit(IEntity entity){}
    public void Enter(IEntity entity){}
	public virtual IEntityState HandleInput(IEntity entity, double delta)
	{
		if (entity.IsOnGround)
		{
			return new GroundState();
		}
		var vel = entity.Velocity;
		if (entity.IsInAir)
		{
			vel.Y += 1;
		}
		entity.Velocity = vel;
		return this;
	}
} 
