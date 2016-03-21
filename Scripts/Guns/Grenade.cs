using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour {
	public Transform explosion;

	public float waitTime = 2.0f;
	public float hitpoints = 50.0f;
	void Start() {
		StartCoroutine (waitanddestroy());
	}
	void update()
	{
		if (hitpoints <= 0.0f) 
		{
			explode();
		}
	}
	void explode() 
	{
		
		Instantiate(explosion, transform.position,transform.rotation);
		Destroy (gameObject);
	}
	IEnumerator waitanddestroy ()
	{
		yield return new WaitForSeconds (waitTime);
		explode ();
	}
	void Damage (float damage) 
	{
		hitpoints = hitpoints - damage;
	}
	
}