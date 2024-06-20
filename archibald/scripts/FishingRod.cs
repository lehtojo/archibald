using Godot;
using System;

public partial class FishingRod : Node3D
{
	public AnimationPlayer Ap { get; set; }
	public bool WaitForThrow { get; set; } = false;
	[Export]
	public PackedScene FloatScene { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		Ap = GetNode<AnimationPlayer>("AnimationPlayer");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
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
		RigidBody3D koho = FloatScene.Instantiate<RigidBody3D>();
		GetNode("/root/World").AddChild(koho);
		Vector3 ps = GetNode<Node3D>("ProjectileSpawn").GlobalPosition;
        koho.Position = ps;
		Vector3 throwDirection = GetNode<Node3D>("ProjectileDirection").GlobalPosition - ps;
        koho.ApplyImpulse(throwDirection.Normalized() * 10);
	}
}
