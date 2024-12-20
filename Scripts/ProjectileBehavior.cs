using Godot;
using System;

public class ProjectileBehavior : Sprite
{
	private static bool _DEBUG = false;
	private static bool _SCREEN_DEBUG = false;

	public Vector2 _velocity;
	public float _DAMAGE;
	public float _SPLASH_RADIUS;

	public void SetValues (Vector2 position, Vector2 velocity, float damage, float splash_radius)
	{
		this.Position = position;
		_velocity = velocity;
		_DAMAGE = damage;
		_SPLASH_RADIUS = splash_radius;
	}

	public override void _Ready()
	{
		
	}

	public override void _PhysicsProcess(float Delta)
	{
		if(_DEBUG)
		{
			GD.Print("projectile velocity: ", _velocity);

		}
		this.Position += Delta*_velocity;
	}

	public override void _Process(float delta)
	{
		Area2D collisionHandler = GetChild<Area2D>(0);
		// GetChildren();
		var collisions = collisionHandler.GetOverlappingAreas();
		foreach(Area2D other in collisions)
		{
			Node parentNode = other.FindParent(".*");
			//TODO update hp

		}
		//initiate some explosion animation?..
		this._ExitTree();
	}
}
