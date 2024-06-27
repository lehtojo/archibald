using Godot;

public partial class FishingRod : Node3D
{
	public AnimationPlayer Ap { get; set; }
	public bool WaitForThrow { get; set; } = false;
	[Export]
	public PackedScene FloatScene { get; set; }
	[Export]
	public Node3D? Tip { get; set; }
	public Koho? Koho { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Ap = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Koho == null)
		{
			if (Input.IsActionPressed("throwRod"))
			{
				if (!WaitForThrow) Ap.Play("readyingAnimation");
			}
			else
			{
				if (WaitForThrow)
				{
					Ap.Play("readyingAnimation");
				}
				else
				{
					Ap.Play("RESET");
					WaitForThrow = false;
				}
			}
		}
		else
		{
			if (Input.IsActionPressed("throwRod"))
			{
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
		Ap.Pause();
	}


	public void RodThrowEnded()
	{
		WaitForThrow = false;
		Ap.Pause();
	}

	public void ThrowFloat()
	{
		// float = koho 
		Koho = FloatScene.Instantiate<Koho>();
		GetNode("/root/World").AddChild(Koho);
		Vector3 ps = GetNode<Node3D>("ProjectileSpawn").GlobalPosition;
		Koho.Position = ps;
		Vector3 throwDirection = GetNode<Node3D>("ProjectileDirection").GlobalPosition - ps;
		Koho.ApplyImpulse(throwDirection.Normalized() * 10);
	}
}
