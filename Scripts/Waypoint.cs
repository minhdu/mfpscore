using UnityEngine;
using System.Collections;

public class Waypoint : MonoBehaviour {

	public bool isPlayer;
	Transform trans;
	public Vector3 Position {
		get {
			if (trans == null)
				trans = GetComponent<Transform> ();
			return trans.position;
		}
	}
}
