using Godot;
using System;
using System.Linq.Expressions;

public partial class player_health : Player
{

	//[Export]
	//public int _health = 30;

	//private SignalBus _damageSignal;
	//private SignalBus _respawnSignal;


	//public enum //HealthState
	//{
		//Alive,
		//Dead,
	//};
	//public HealthState Health;




	// Called when the node enters the scene tree for the first time.
	//public override void _Ready()
	//{
		//_damageSignal = GetNode<SignalBus>("/root/SignalBus");
		//_damageSignal.Damage += Playerdamage;
		//_respawnSignal = GetNode<SignalBus>("/root/SignalBus");
		//_respawnSignal.Respawn += Playerrespawn;
		
	//}




	//private void Playerdamage(int damageAmount)
	//{
		//_health -= damageAmount;

		//if (_health <= 0)
		//{
			//Health = HealthState.Dead;
		//}
		//else
		//{
			//Health = HealthState.Alive;
		//}
		//GD.Print("", Health);
	//}

	//private void Playerrespawn(bool reset)
	//{
		//_health = 30;
		//Health = HealthState.Alive;
	//}



}
