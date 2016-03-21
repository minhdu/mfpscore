using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour {
	public float speed = 100f;
	public GameObject explosion;
	public float waitTime = 5.0f;
	// Use this for initialization
	void Start () {
		Destroy (gameObject, waitTime);
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		GetComponent<Rigidbody>().AddRelativeForce(0f,0f,speed);
	
	}
	void OnCollisionEnter(){
		Instantiate(explosion, transform.position, Quaternion.identity);
		Destroy(gameObject); // destroy the projectile
		//Destroy(expl, 3); // delete the explosion after 3 seconds
	}
}
