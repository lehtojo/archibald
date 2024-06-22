using Godot;
using System;

public partial class Water : MeshInstance3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public float GetHeight(Vector3 position)
	{
		// TODO: Wave hight calculation
		return 1.0f;
	}
}
