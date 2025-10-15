using Godot;
using System;

public partial class Respawn : Button
{

	private SignalBus _respawnSignal;
	bool _respawning;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_respawnSignal = GetNode<SignalBus>("/root/SignalBus");
	}

    public override void _Pressed()
    {
		_respawnSignal.EmitSignal(nameof(SignalBus.Respawn), _respawning);
    }


}
