using Godot;
using System;

public interface IMovementEntity
{
    Vector2 Velocity { get; set; }
    float SpeedMulti { get; set; }
    float JumpForce { get; set; }
    float BaseGravity { get; set; }
    float CurrentGravity { get; set; }

    Vector2 MyPosition {get;}
    CollisionShape2D MyShape {get;}

    

    bool IsOnGround { get; }
    bool IsInAir { get; }
    bool IsTouchingLeft { get; }
    bool IsTouchingRight { get; }

    void HandleMovement(Vector2 direction, double delta);

    void HandleJumping(double delta);

    void HandleGravity(double delta);
}