using UnityEngine;
using System.Collections;

public class AutoLookAt : MonoBehaviour {

	Transform mTransform;

	void Awake () {
		mTransform = GetComponent<Transform> ();
	}

	// Update is called once per frame
	void Update () {
		mTransform.LookAt (PlayerController.Instance.HitPoint);
	}
}
