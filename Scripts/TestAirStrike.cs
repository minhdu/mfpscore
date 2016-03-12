using UnityEngine;
using System.Collections;

public class TestAirStrike : MonoBehaviour {

	// Use this for initialization
	public void Active () {
		AirStrike.Instance.Active (transform.position);
	}
}
