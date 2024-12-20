using Godot;
using System;

public static class Utilities
{
    public static int degrees_rotation_from_radians(float rotation_radians, int degrees_increment)
	{
		int rotation = (int)(rotation_radians*(180f/(float)Math.PI)+degrees_increment/2);
		rotation = (rotation + 360+90)%360;
		rotation = (rotation/degrees_increment)*degrees_increment;
		return rotation;
	}
}