using System;
using Godot;

public partial class FishingSystem : Node
{
    [Export]
    public int SectorCount { get; set; } = 8;
    public float DegreesPerSector => 360.0f / SectorCount;

    [Export]
    public Vector2I CorrectSectorWaitTime { get; set; } = new(30, 60);

    [Export]
    public Vector2I IncorrectSectorWaitTime { get; set; } = new(120, 240);

    [Export]
    public bool IsDebugDrawEnabled { get; set; } = false;

    [Export]
    public float DebugSectorBallRadius { get; set; } = 1.0f;

    [Export]
    public float DebugSectorBallDistance { get; set; } = 20.0f;

    [Export]
    public float DebugSectorBallY { get; set; } = 5.0f;

    public int CorrectSector { get; private set; } = -1;

    public void ChooseNextSector()
    {
        CorrectSector = Utils.Random(SectorCount);
    }

    public int GetSectorIndex(Vector3 position)
    {
        var angle = MathF.Atan2(position.Z, position.X) * 180 / MathF.PI;

        // If the angle goes past 180 degrees, the returned angle is represented with negative degrees.
        // We want the angle within [0, 360].
        if (angle < 0)
        {
            angle = 360 + angle;
        }

        return (int)(angle / DegreesPerSector);
    }

    public override void _Ready()
    {
        ChooseNextSector();
    }

    public override void _Process(double delta)
    {
        if (!IsDebugDrawEnabled)
        {
            return;
        }

        for (var sector = 0; sector < SectorCount; sector++)
        {
            var color = sector == CorrectSector ? Colors.Green : Colors.Red;
            var angle = (sector * DegreesPerSector + 0.5f * DegreesPerSector) * MathF.PI / 180.0f;

            var position = new Vector3(MathF.Cos(angle), DebugSectorBallY, MathF.Sin(angle));
            position *= DebugSectorBallDistance;

            DebugDraw3D.DrawSphere(position, DebugSectorBallRadius, color);
        }
    }
}
