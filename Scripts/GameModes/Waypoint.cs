using UnityEngine;
using System.Collections;

[System.Serializable]
public class Waypoint : MonoBehaviour {

	public bool isPlayer;
	public bool isWeaker;
	Transform trans;

	void Awake () {
		if (trans == null)
			trans = GetComponent<Transform> ();
	}

	public Vector3 Position {
		get {
			return trans.position;
		}
	}
}
