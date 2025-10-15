using Godot;
using GodotPlugins.Game;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


using System.Reflection;






public partial class Hook : Node2D
{
	private int speed { get; set; } = 400;
	private int returnspeed { get; set; } = 650;
	private int AreaCheck;
	private Vector2 _target;

	private SignalBus _parentalive;
	private SignalBus _hitcheck;

	bool parentlives;
	bool outside;

	private Vector2 velocity;


	private PinJoint2D HookPin;

	public enum HitID
	{
		Walls,
		Enemies
	}
	private Area2D hookshape;






	public override void _Ready()
	{
		_parentalive = GetNode<SignalBus>("/root/SignalBus");
		_hitcheck = GetNode<SignalBus>("/root/SignalBus");
		hookshape = GetNode<Area2D>("Hook2DArea");
		//hookshape.BodyShapeEntered += HookHitCheck;




		_parentalive.Alive += _alive;
		parentlives = true;
		outside = false;


		

		var MaxRadiusAreaexit = (HookRadius)GetNode("../HookRadius");
		MaxRadiusAreaexit.AreaExited += OnMaxRadiusAreaExit;
		MaxRadiusAreaexit.AreaEntered += OnMaxRadiusAreaEnter;







		Hide();



	}






	// Called every frame. 'delta' is the elapsed time since the previous frame.


	public override void _PhysicsProcess(double delta)
	{

		if (parentlives)
		{
			velocity = Vector2.Zero;


			if (Input.IsMouseButtonPressed(MouseButton.Right))
			{

				_GetTarget();
				velocity = _target.Normalized() * speed;
				Show();

				//if HIT
				// velocity = Vector2.Zero;


			}
			if (!Input.IsMouseButtonPressed(MouseButton.Right) && Position.DistanceTo(-Position) > 40)
			{

				velocity = -Position.Normalized() * returnspeed;


				if (Position.DistanceTo(-Position) < 70)
				{
					Hide();

				}


			}






			//wenn außerhalb von Hookradius bounce 
			if (outside)
			{
				velocity = -Position.Normalized() * returnspeed;
			}

			//move
			Position += velocity * (float)delta;


			//if inside player reset position:




		}
	}


	private void _GetTarget()
	{
		_target = GetLocalMousePosition();

	}
	private void _alive(bool what)
	{
		if (what)
		{
			parentlives = true;
		}
		if (!what)
		{
			parentlives = false;
		}

	}
	private void OnMaxRadiusAreaExit(Area2D Exit)
	{
		outside = true;
	}
	private void OnMaxRadiusAreaEnter(Area2D Enter)
	{
		outside = false;
	}


	private void HookHitCheck()
	{
		
	}



























	











} 
