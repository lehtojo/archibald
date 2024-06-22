using Godot;
using System;

public partial class Koho : RigidBody3D
{
	[Export]
	public float BoyancyForce { get; set; } = 1.0f;
	[Export]
	public float WaterDrag { get; set; } = 0.85f;
	[Export]
	public float WaterAngularDrag { get; set; } = 0.85f;
	[Export]
	public Water? Water { get; set; }
    public float Gravity { get; set; }
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
		Submerged = false;
        float waterLevel = Water.GetHeight(GlobalPosition);
		float depth = waterLevel - this.GlobalPosition.Y;
		if (depth > 0f)
		{
			Submerged = true;
            ApplyCentralForce(Vector3.Up * -depth * BoyancyForce * Gravity);
		}
    }

    public override void _IntegrateForces(PhysicsDirectBodyState3D state)
    {
		if (Submerged)
		{
            state.LinearVelocity *= 1 - WaterDrag;
            state.AngularVelocity *= 1 - WaterAngularDrag;
        }
    }
}
