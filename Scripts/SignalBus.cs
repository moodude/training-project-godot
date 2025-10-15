using Godot;
using System;

public partial class SignalBus : Node
{
    [Signal]
    public delegate void DamageEventHandler(int damageAmount);
    [Signal]
    public delegate void RespawnEventHandler();
    [Signal]
    public delegate void AliveEventHandler(bool alive);
    [Signal]
    public delegate void RayCastDirectionEventHandler(Vector2 direction);
    [Signal]
    public delegate void HookHitDetectionEventHandler(bool Hit);
    
       
}
   


