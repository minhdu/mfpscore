using UnityEngine;

public class SpawnPoint : MonoBehaviour {

	Transform mTransform;

	public Vector3 Position {
		get{
			return mTransform.position;
		}
	}

	public Quaternion Rotation {
		get{
			return mTransform.rotation;
		}
	}

	void Awake () {
		mTransform = GetComponent<Transform> ();
	}
}
