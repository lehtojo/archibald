using Godot;

public partial class FishingSystem : Node
{
    [Export]
    public int SectorCount { get; set; } = 8;

    [Export]
    public Vector2I CorrectSectorWaitTime { get; set; } = new(30, 60);

    [Export]
    public Vector2I IncorrectSectorWaitTime { get; set; } = new(120, 240);

    private int CorrectSector { get; set; } = -1;

    public void ChooseNextSector()
    {
        CorrectSector = (int)GD.Randi() % SectorCount;
    }

    public override void _Ready()
    {
        // Choose the sector when the game starts
        ChooseNextSector();
    }
}
