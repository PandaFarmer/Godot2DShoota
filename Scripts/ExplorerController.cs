using Godot;
using System;
using System.Collections.Generic;

public class ExplorerController : Sprite
{
	private static bool _DEBUG = true;
	private static bool _SCREEN_DEBUG = false;

	private static float _MAX_VElOCITY = 120f;
	private static float _MAX_ACCELERATION = 30f;

	private Vector2 _velocity = new Vector2();

	private static float _INERTIAL_DAMPING = 2f;

	//remove?
	private static float _MAX_DIRECTIONAL_VElOCITY = 10f;
	private static float _MAX_DIRECTIONAL_ACCELERATION = 2f;

	private static float _TURNRATE = 5f;//degrees? also might leave this out for snappier controls
	
	public float _rotation_radians;
	public float _rotation_degrees;

	private static string _TEXTURE_PATH = "res://ExplorerTextures/Explorer";
	private static string _TEXTURE_PATH_PREFIX = "isometric";
	private static int _DEGREES_INCREMENT_SPRITE_ROTATION = 30;

	private Dictionary<string, Texture> _sprite_textures = new Dictionary<string, Texture>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Loads Sprite Textures:
		LoadSpriteTextures();
		_rotation_radians = 0;
		if(_DEBUG)
		{
			GD.Print("PI: ", Mathf.Atan2(0f, -1f));
			GD.Print("Zero: ", Mathf.Atan2(0f, 1f));
			GD.Print("PI/2: ", Mathf.Atan2(1f, 0f));
			GD.Print("-PI/2: ", Mathf.Atan2(-1f, 0f));
		}
	}

	//
	public override void _Input(InputEvent @event)
	{
		UpdateRotation(@event);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
 	public override void _Process(float delta)
 	{
 	   this.Position += _velocity*delta;
 	}

	//polling important for continuous input
	public override void _PhysicsProcess(float delta)
	{
		// Mouse in viewport coordinates
		// Print the size of the viewport
		if(_SCREEN_DEBUG)
			GD.Print("Viewport Resolution is: ", GetViewportRect().Size);
		UpdateVelocity();
		UpdateSpriteTexture();
	}

	public void LoadSpriteTextures()
	{
		string texture_path;
		Texture currImage;
		for(int rotation = 0; rotation < 360; rotation += _DEGREES_INCREMENT_SPRITE_ROTATION)
		{
			texture_path = _TEXTURE_PATH + _TEXTURE_PATH_PREFIX + (string)"_" + Convert.ToString(rotation) + (string)".png";
			currImage = GD.Load<Texture>(texture_path);
			_sprite_textures.Add(Convert.ToString(rotation), currImage);
		}
	}

	public void UpdateRotation(InputEvent @event)
	{
		if (@event is InputEventMouseButton eventMouseButton)
		{
			if(_DEBUG)
				GD.Print("Mouse Click/Unclick at: ", eventMouseButton.Position);
		}

		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			if(_DEBUG)
			{
				GD.Print("Mouse Motion at: ", eventMouseMotion.Position);
				GD.Print("Player Current Position: ", this.Position);
			}
				
			Vector2 diffV = eventMouseMotion.Position - this.Position;
			// _rotation_radians = Mathf.Atan2(diffV.y, diffV.y);//idk y this doesn't work, maybe write custom estimator
			_rotation_radians = Mathf.Atan2(diffV.y, diffV.x);
			if(_DEBUG)
			{
				GD.Print("_rotation_radians: ", _rotation_radians);
				GD.Print("diffV: ", diffV);
			}
		}

		_rotation_degrees = Utilities.degrees_rotation_from_radians(_rotation_radians, _DEGREES_INCREMENT_SPRITE_ROTATION);
	}

	// public float Atan2_90s(float y, float x)
	// {
	// 	float yAbs = Math.Abs(y);
	// 	float xAbs = Math.Abs(x);
	// 	if(yAbs > xAbs)
	// 	{
	// 		if(y > 0)
	// 			return (float)(Math.PI/2f);
	// 		return -(float)(Math.PI/2f);
	// 	}
	// 	if(x > 0)
	// 			return 0f;
	// 		return (float)Math.PI;
	// }

	public void UpdateSpriteTexture()
	{
		int degrees_rotation = Utilities.degrees_rotation_from_radians(_rotation_radians, _DEGREES_INCREMENT_SPRITE_ROTATION);

		this.Texture = _sprite_textures[Convert.ToString(degrees_rotation)];
	}

	public void UpdateVelocity()
	{
		float net_x = 0;
		float net_y = 0;
		if (Input.IsActionPressed("ui_right"))
			net_x += 1;
		if (Input.IsActionPressed("ui_left"))
			net_x -= 1;
		if (Input.IsActionPressed("ui_up"))
			net_y -= 1;
		if (Input.IsActionPressed("ui_down"))
			net_y += 1;

		if (net_y == 0 && net_x == 0)
		{
			_velocity = _velocity.Normalized()*Math.Max(_velocity.Length()-_INERTIAL_DAMPING, 0f);
			return;
		}
		Vector2 acceleration_v = new Vector2 (net_x, net_y);
		acceleration_v = acceleration_v.Normalized();
		acceleration_v = acceleration_v*_MAX_ACCELERATION;
		// float acceleration_magnitude = Math.min();
		_velocity = _velocity + acceleration_v;
		_velocity = _velocity.Normalized()*Math.Min(_velocity.Length(), _MAX_VElOCITY);
	}

	public float ClampDirectionalV(float v)
	{
		return Mathf.Clamp(v, -_MAX_DIRECTIONAL_VElOCITY, _MAX_DIRECTIONAL_VElOCITY);
	}

	public void UpdateVelocityDirectional()
	{
		float net_x = 0;
		float net_y = 0;

		if (Input.IsActionPressed("ui_right"))
			net_x += _MAX_DIRECTIONAL_ACCELERATION;
		if (Input.IsActionPressed("ui_left"))
			net_x -= _MAX_DIRECTIONAL_ACCELERATION;
		if (Input.IsActionPressed("ui_up"))
			net_y += _MAX_DIRECTIONAL_ACCELERATION;
		if (Input.IsActionPressed("ui_down"))
			net_y -= _MAX_DIRECTIONAL_ACCELERATION;

		float new_x = _velocity.x + net_x;
		float new_y = _velocity.y + net_y;
		_velocity = new Vector2(ClampDirectionalV(new_x), ClampDirectionalV(new_y));
	}

 	
}
