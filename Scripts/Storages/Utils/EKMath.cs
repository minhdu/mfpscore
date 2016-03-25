using UnityEngine;
using System.Collections;

public struct CircleBrush 
{
	public GameObject Pointer {get; set;}
	public Vector2 Center {get; set;}
	public float Radius {get; set;}

	public CircleBrush (Vector2 center, float radius) {
		this.Center = center;
		this.Radius = radius;
	}

	public bool Contain (Vector2 target) {
		return (Mathf.Pow(target.x - Center.x,2) + Mathf.Pow(target.y - Center.y,2) < Mathf.Pow(Radius,2));
	}
}

public class EKMath {

}