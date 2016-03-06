using UnityEngine;
using System.Collections;

public class AutoDestruct : MonoBehaviour {

	public bool isPoolale = false;
	public PoolTag poolTag = PoolTag.NONE;
	public float delay = 0;

	// Use this for initialization
	IEnumerator Start () {
		yield return new WaitForSeconds (delay);

		if (isPoolale) {
		} else {
			Destroy (gameObject);
		}
	}
}
