using UnityEngine;
using System.Collections;

public class AirStrike : Singleton<AirStrike> {

	public int bulletPerStep = 6;
	public Vector2 spreadRange = new Vector2(5,2);
	public GameObject projectilePrefab;
	public Transform strikeDestination;
	public Vector3 bulletLineAngle = new Vector3(45,180,0);
	public float flyDistance;

	Transform mTransform;
	Vector3 _targetPosition;

	void Awake () {
		mTransform = GetComponent<Transform> ();
	}

	public void Active (Vector3 targetPosition) {
		_targetPosition = targetPosition;
		LeanTween.move (gameObject, strikeDestination.position, flyDistance).setOnUpdate(Shoot);
	}

	void Shoot (Vector3 position) {
		for(int j=0; j<bulletPerStep; j++) {
			Vector3 bulletPosition = position;
			bulletPosition.x += Random.Range (-spreadRange.x, spreadRange.x);
			bulletPosition.z += Random.Range (-spreadRange.y, spreadRange.y);
			Instantiate (projectilePrefab, bulletPosition, Quaternion.Euler (bulletLineAngle));
		}
	}
}
