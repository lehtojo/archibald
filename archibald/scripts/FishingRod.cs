using Godot;

public partial class FishingRod : Node3D
{
	public AnimationPlayer Animations { get; set; } = null!;
	public bool WaitForThrow { get; set; } = false;

	[Export]
	public PackedScene? FloatScene { get; set; }

	[Export]
	public float FloatThrowImpulse { get; set; } = 50.0f;

	[Export]
	public Node3D? Tip { get; set; }

	[Export]
	public PackedScene[] Fishes { get; set; } = [];

	public Koho? Koho { get; set; }
	public PointSystem Points { get; set; }

	private void OnFishCaught()
	{
		Points.AddPoint();
        GD.Print("Caught the fish :^)");
	}

	private void CatchFish()
	{
		GD.Print("Catching a fish...");

		if (Koho == null)
		{
			GD.PrintErr("Koho is not set");
			return;
		}

		Koho.CatchFish();

		// Choose a fish to catch and spawn it under the float
		var fish = Utils.Random(Fishes).Instantiate<FishItem>();
		GetNode("/root/World").AddChild(fish);

		fish.Start = Koho.GlobalPosition + Vector3.Up;
		fish.Target = this;
		fish.Received += OnFishCaught;
	}

	public override void _Ready()
	{
		Animations = GetNode<AnimationPlayer>("AnimationPlayer");
		Points = GetNode<PointSystem>("/root/World/PointSystem");
    }

	public override void _Process(double delta)
	{
		if (Koho == null)
		{
			if (Input.IsActionPressed("throwRod"))
			{
				if (!WaitForThrow) Animations.Play("readyingAnimation");
			}
			else
			{
				if (WaitForThrow)
				{
					Animations.Play("readyingAnimation");
				}
				else
				{
					Animations.Play("RESET");
					WaitForThrow = false;
				}
			}
		}
		else
		{
			if (Input.IsActionPressed("throwRod"))
			{
				if (Koho.IsFishCaught)
				{
					CatchFish();
				}

				Koho.QueueFree();
				Koho = null;
			}
		}

		if (Tip != null && Koho != null)
		{
			DebugDraw3D.DrawLine(Tip.GlobalPosition, Koho.GlobalPosition, Colors.Black);
		}
	}

	public void RodReachedTop()
	{
		WaitForThrow = true;
		Animations.Pause();
	}

	public void RodThrowEnded()
	{
		WaitForThrow = false;
		Animations.Pause();
	}

	public void ThrowFloat()
	{
		if (FloatScene == null)
		{
			GD.PrintErr("Float scene is not set");
			return;
		}

		// float = koho 
		Koho = FloatScene.Instantiate<Koho>();
		GetNode("/root/World").AddChild(Koho);

		// Place the float in the world and throw it
		var position = GetNode<Node3D>("ProjectileSpawn").GlobalPosition;
		var throw_direction = GetNode<Node3D>("ProjectileDirection").GlobalPosition - position;

		Koho.Position = position;
		Koho.ApplyImpulse(throw_direction.Normalized() * FloatThrowImpulse);
	}
}
