using Godot;
using System;

public partial class Archibald : RigidBody3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<AnimationPlayer>("ArchiModel/AnimationPlayer").Play("Sitting_001");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
