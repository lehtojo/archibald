using Godot;

public partial class Boat : Node3D
{
    [Export]
    public float RollSpeed { get; set; } = 0.1f;

    [Export]
    public float MaxRoll { get; set; } = 3.0f;

    private float Time { get; set; }

    public override void _Process(double delta)
    {
        Time += (float)delta;
        var roll = Mathf.Sin(Time) * MaxRoll;
        
        RotationDegrees = new Vector3(
            RotationDegrees.X, RotationDegrees.Y, roll
        );
    }
}
