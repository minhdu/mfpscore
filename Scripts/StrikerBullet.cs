using UnityEngine;
using System.Collections;

public class StrikerBullet : MonoBehaviour {

	public float velocity = 0.25f;
	Transform mTransform;
	public GameObject impactPrefab;
	Collider mCollider;

	void Awake () {
		mTransform = GetComponent<Transform> ();
		mCollider = GetComponent<BoxCollider> ();
	}

	void Update () {
		mTransform.Translate (Vector3.forward*velocity);
	}

	void OnCollisionEnter (Collision col) {
		Instantiate(impactPrefab,  col.contacts [0].point, Quaternion.identity);
		mCollider.enabled = false;
		Destroy (gameObject);
	}
}
