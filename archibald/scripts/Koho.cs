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
	[Export]
	public CpuParticles3D? CatchingParticles { get; set; }
	[Export]
	public Timer? ReleaseTimer { get; set; }
	[Export]
	public int ReleaseTime { get; set; } = 3;

	public int? Sector => FishingSystem?.GetSectorIndex(GlobalPosition);

	private FishingSystem? FishingSystem { get; set; }
	private float BuoancyTimeElapsed { get; set; } = 0.0f;
	private bool Submerged { get; set; } = false;
	public bool IsFishCaught { get; private set; }

	private void EnableCatchingAnimation()
	{
		if (CatchingParticles == null)
		{
			GD.PrintErr("Catching particles is not set");
			return;
		}

		CatchingParticles.Emitting = true;

		// Todo: The float could also shake up and down?
	}

	private void DisableCatchingAnimation()
	{
		if (CatchingParticles == null)
		{
			GD.PrintErr("Catching particles is not set");
			return;
		}

		CatchingParticles.Emitting = false;
	}

	public void CatchFish()
	{
		FishingSystem?.ChooseNextSector();
	}

	public void OnFishReleased()
	{
		GD.Print("Fish released");
		IsFishCaught = false;
		DisableCatchingAnimation();
	}

	private void ReleaseFishAfter(int seconds)
	{
		if (ReleaseTimer == null)
		{
			GD.PrintErr("Release timer is not set");
			return;
		}

		GD.Print($"Releasing fish in {seconds} second(s)...");

		ReleaseTimer.WaitTime = seconds;
		ReleaseTimer.Start();
	}

	public void OnFishCaught()
	{
		GD.Print("Fish took the bait");
		EnableCatchingAnimation();

		// Release the fish shortly if the player doesn't respond
		IsFishCaught = true;
		ReleaseFishAfter(ReleaseTime);
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

		var sector = Sector;

		if (sector == null || FishingSystem == null)
		{
			GD.PrintErr("Fishing system is not set");
			return;
		}

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