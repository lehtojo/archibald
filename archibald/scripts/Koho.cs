using Godot;
using System;

public partial class Koho : RigidBody3D
{
	[Export]
	public float BuoyancyRecoveryTime { get; set; } = 1.0f;
	[Export]
	public float WaterDrag { get; set; } = 0.85f;
	[Export]
	public float WaterAngularDrag { get; set; } = 0.85f;
	[Export]
	public float WaterLevel { get; set; } = 0.0f;
	[Export]
	public float BuoyancyStart { get; set; } = -5.0f;
	[Export]
	public Water? Water { get; set; }
	public float Gravity { get; set; }
	private float BuoancyTimeElapsed { get; set; } = 0.0f;
	private bool Submerged { get; set; } = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Gravity = -(float)ProjectSettings.GetSetting("physics/3d/default_gravity");
		Water = GetNode<Water>("/root/World/Water");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		if (GlobalPosition.Y <= BuoyancyStart)
		{
			Submerged = true;
			BuoancyTimeElapsed = 0.0f;
		}

		if (Submerged)
		{
			BuoancyTimeElapsed += (float)delta;

			var buoancy = Mathf.Min(BuoancyTimeElapsed / BuoyancyRecoveryTime, 1.0f);
			GlobalPosition = GlobalPosition.Lerp(new Vector3(GlobalPosition.X, WaterLevel, GlobalPosition.Z), buoancy);

			Freeze = true;
		}
	}
}
