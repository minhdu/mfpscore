using UnityEngine;
using System.Collections;

public class Lookable : MonoBehaviour {

	Transform mTransform;

	void Awake () {
		mTransform = GetComponent<Transform> ();
	}

	public void Look (Vector3 position) {
		gameObject.SetActive (true);
		mTransform.LookAt (position);
		Unlook ();
	}

	public void Unlook () {
		StartCoroutine (DelayUnloook ());
	}

	IEnumerator DelayUnloook () {
		yield return new WaitForSeconds (Random.Range(0.05f, 0.125f));
		if (gameObject.activeSelf)
			gameObject.SetActive (false);
	}
}
