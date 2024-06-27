using System;
using Godot;

public partial class FishItem : Node3D
{
    public Vector3 Start { get; set; } = Vector3.Zero;

    [Export]
    public Node3D? Target { get; set; }

    [Export]
    public float RotationSpeed { get; set; } = Mathf.Pi;

    [Export]
    public float ReceiveTime { get; set; } = 2.0f;

    public event Action? Received;

    private float Elapsed { get; set; }

    public override void _Process(double delta)
    {
        if (Target != null)
        {
            Elapsed += (float)delta;
            GlobalPosition = Start.Lerp(Target.GlobalPosition, Elapsed / ReceiveTime);
        }

        RotateY(RotationSpeed * (float)delta);

        if (Elapsed >= ReceiveTime)
        {
            Received?.Invoke();
            Free();
        }
    }
}