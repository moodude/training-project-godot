using Godot;
using System;
using System.Net.Http.Headers;

public partial class healthbar : ProgressBar
{
	private SignalBus _customSignals;

    public override void _Ready()
    {
        _customSignals = GetNode<SignalBus>("/root/SignalBus");
		_customSignals.Damage += HealthUpdate;
        _customSignals.Respawn += HealthReset;
    }

   public void HealthUpdate (int damageAmount)
   {		
		Value -= damageAmount;		
   }

   public void HealthReset()
   {
   }



}
