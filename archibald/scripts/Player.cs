using Godot;
using System;

public partial class Player : CharacterBody3D
{
	public const float Speed = 3.0f;
	public const float JumpVelocity = 3.5f;
	public const float CameraSensitivity = 0.2f;

	[Export]
	public Node3D RotationHelper { get; set; }

	[Export]
	public FishingRod? Rod { get; set; }

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public int? FloatSector => Rod?.FloatSector;

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion ev)
		{
			RotateY(-MathF.PI / 180f * ev.Relative.X * CameraSensitivity);
			RotationHelper.RotateX(-MathF.PI / 180f * ev.Relative.Y * CameraSensitivity);
			RotationHelper.RotationDegrees = new Vector3(Mathf.Clamp(RotationHelper.RotationDegrees.X, -89, 89),
				RotationHelper.RotationDegrees.Y,
				RotationHelper.RotationDegrees.Z);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("left", "right", "forward", "backward");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
