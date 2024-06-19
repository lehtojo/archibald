using Godot;
using System;

public partial class FishingRod : Node3D
{
	public AnimationPlayer Ap { get; set; }
	public bool WaitForThrow { get; set; } = false;

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
}
