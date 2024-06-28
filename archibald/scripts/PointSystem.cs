using Godot;
using System;
using System.Collections.Generic;

public partial class PointSystem : Node3D
{
	[Export]
    public PackedScene PointScene { get; set; }
	public List<MeshInstance3D> Points { get; set; } = new List<MeshInstance3D>();
    public RandomNumberGenerator Rng { get; set; } = new RandomNumberGenerator();

    public void AddPoint()
    {
        MeshInstance3D point = PointScene.Instantiate<MeshInstance3D>();
        AddChild(point);
        point.Position = new Vector3(0, 
            -0.253f + Points.Count * 0.05f,
            0);
        point.RotateY(Rng.RandfRange(0, 6.28f));
        Points.Add(point);
    }
}
