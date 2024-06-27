using System;
using Godot;

public partial class Koho : RigidBody3D
{
	[Export]
	public float BuoyancyRecoveryTime { get; set; } = 1.0f;
	[Export]
	public float WaterLevel { get; set; } = 0.0f;
	[Export]
	public float BuoyancyStart { get; set; } = -5.0f;
	[Export]
	public Timer? CatchTimer { get; set; }

	public event Action? Caught;

	private FishingSystem? FishingSystem { get; set; }
	private float BuoancyTimeElapsed { get; set; } = 0.0f;
	private bool Submerged { get; set; } = false;

	public void OnFishCaught()
	{
		GD.Print("Catching a fish...");
		Caught?.Invoke();
		FishingSystem?.ChooseNextSector();
	}

	private void CatchFishAfter(int seconds)
	{
		if (CatchTimer == null)
		{
			GD.PrintErr("Catch timer is not set");
			return;
		}

		GD.Print($"Catching fish in {seconds} second(s)...");

		CatchTimer.WaitTime = seconds;
		CatchTimer.Start();
	}

	private void OnSubmerged()
	{
		BuoancyTimeElapsed = 0.0f;

		if (FishingSystem == null)
		{
			GD.PrintErr("Fishing system is not set");
			return;
		}

		var sector = FishingSystem.GetSectorIndex(GlobalPosition);
		var correct = sector == FishingSystem.CorrectSector;

		if (correct)
		{
			GD.Print("Player threw the float in the correct sector");
		}
		else
		{
			GD.Print("Player threw the float in wrong sector");
		}

		GD.Print($"Correct Sector = {FishingSystem.CorrectSector}, Float Sector = {sector}, Angle per Sector = {FishingSystem.DegreesPerSector}");

		var wait_time = correct
			? Utils.Random(FishingSystem.CorrectSectorWaitTime)
			: Utils.Random(FishingSystem.IncorrectSectorWaitTime);

		CatchFishAfter(wait_time);
	}

	public override void _Ready()
	{
		FishingSystem = GetNode<FishingSystem>("/root/World/Fishing System");

		if (FishingSystem == null)
		{
			GD.PrintErr("Failed to find the fishing system");
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Submerged)
		{
			BuoancyTimeElapsed += (float)delta;

			var buoancy = Mathf.Min(BuoancyTimeElapsed / BuoyancyRecoveryTime, 1.0f);
			GlobalPosition = GlobalPosition.Lerp(new Vector3(GlobalPosition.X, WaterLevel, GlobalPosition.Z), buoancy);

			Freeze = true;
			return;
		}

		if (GlobalPosition.Y <= BuoyancyStart)
		{
			Submerged = true;
			OnSubmerged();
		}
	}
}