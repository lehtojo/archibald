using Godot;

public partial class Water : MeshInstance3D
{
	public override void _Process(double delta) { }

	public float GetHeight(Vector3 position)
	{
		return 0.0f;
	}
}
