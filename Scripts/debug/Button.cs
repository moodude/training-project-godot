using Godot;
using System;

public partial class Button : Godot.Button
{
	private SignalBus _customSignals;
	[Export]
	public int ButtonDamage = 10;



	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		_customSignals = GetNode<SignalBus>("/root/SignalBus");
	}

    public override void _Pressed()
    {
	   _customSignals.EmitSignal(nameof(SignalBus.Damage), ButtonDamage);
    }




}
