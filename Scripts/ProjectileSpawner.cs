using Godot;
using System;

public class ProjectileSpawner : Node
{
	private static bool _DEBUG = true;
	private static bool _SCREEN_DEBUG = false;

	private PackedScene _spriteScene;

	public float _VELOCITY_MAGNITUDE = 200f;
	public float _FIRE_RATE = 3f;//per second ofc

	public float projectile_scale = .05f;

	public override void _Ready()
	{
		_spriteScene = ResourceLoader.Load<PackedScene>("res://Spawnables/Missile16.tscn");
	}
	
	public void SpawnProjectile(float radians_rotation, float degrees_rotation, Vector2 position)
	{
		//keep in mind that projectiles should have a structure that looks like
		//Sprite->Area2D->CollisionShape2D
		var projectileScene = GD.Load<PackedScene>("res://Spawnables/Projectile.tscn"); // Will load when the script is instanced.
		ProjectileBehavior projectileBehavior = (ProjectileBehavior)projectileScene.Instance();
		projectileBehavior.Scale = new Vector2(projectile_scale, projectile_scale);	
		Vector2 velocity = new Vector2((float)Math.Cos(radians_rotation), (float)Math.Sin(radians_rotation));
		if(_DEBUG)
		{
			GD.Print("velocity unit: ", velocity);
		}
		velocity = _VELOCITY_MAGNITUDE*velocity;
		projectileBehavior.SetValues(position, velocity, 10f, 0f);
		
		

		// Load a texture for the sprite
		Texture texture = GD.Load<Texture>("res://missile16Textures/missile16isometric_" + Convert.ToString(degrees_rotation) +".png");
		projectileBehavior.Texture = texture;

		projectileBehavior.Position = position;
		// ProjectileBehavior._EnterTree();
		AddChild(projectileBehavior);
	}

	public override void _Input(InputEvent @event)
	{
		if(@event is InputEventMouseButton eventMouseButton)
		{
			if(!eventMouseButton.Pressed)
			{
				return;
			}
			Node parentNode = this.GetParent();
			if(! (parentNode is ExplorerController explorerController))
			{
				GD.Print("cannot convert parentNode of ProjectileSpawner to ExplorerController");
			}
			else
			{
				float ecRot = explorerController._rotation_radians;
				Vector2 spawnPosition = new Vector2((float)Math.Cos(ecRot), (float)Math.Sin(ecRot));
				spawnPosition += explorerController.Position;
				SpawnProjectile(ecRot, explorerController._rotation_degrees, spawnPosition);
			}
		}
	}
}

