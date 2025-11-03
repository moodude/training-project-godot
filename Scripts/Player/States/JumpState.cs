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
		if (entity.IsOnGround && entity.Velocity.Y >= 0)
		{
			GD.Print($"[JumpState] Detected ground. Velocity.Y={entity.Velocity.Y}");
			return new GroundState();
		}

		
		var vel = entity.Velocity;
		vel.Y += entity.CurrentGravity * (float)delta;
		entity.Velocity = vel;
		
		 GD.Print($"[JumpState] Gravity applied. VelY={entity.Velocity.Y}, Gravity={entity.CurrentGravity}");

		return this;
	}
} 
