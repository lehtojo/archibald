using Godot;
using System;

public partial class GameOver : CanvasLayer
{
	[Export]
	public Camera3D Camera { get; set; }
	[Export]
	public PointSystem PointSystem { get; set; }
	[Export]
	public Label EndingText { get; set; }
	[Export]
	public float WaterLevel { get; set; } = 0f;
	public bool GameEnded { get; set; } = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Camera = GetNode<Camera3D>("/root/World/Player/RotationHelper/Camera3D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Camera.GlobalPosition.Y < WaterLevel)
		{
			if (!GameEnded)
			{
				Visible = true;
				GameEnded = true;
				EndingText.Text += $"Caught {PointSystem.Points.Count} fish";
			}
		}
	}
}
