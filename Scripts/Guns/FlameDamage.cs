using UnityEngine;
using System.Collections;

public class FlameDamage : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnParticleCollision (GameObject other ) {
		other.SendMessageUpwards("Damage", 5f,SendMessageOptions.DontRequireReceiver);
	}
}
