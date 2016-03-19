using UnityEngine;
using System.Collections;

public class arrow : MonoBehaviour {
	public float speed = 100f;
	public float damage = 50.0f;



	
	// Update is called once per frame
	void Update () 
	{
		if (GetComponent<Rigidbody>() != null)
		{
			GetComponent<Rigidbody>().AddRelativeForce(0f,0f,speed);
		}
	
	}
	void OnCollisionEnter(Collision collision){

		Vector3 fwrd = transform.forward;
		ContactPoint contact = collision.contacts[0];
		Collider hit = collision.collider;
		transform.parent = hit.transform;
		transform.position = contact.point;
		if(hit.GetComponent<Rigidbody>()) hit.GetComponent<Rigidbody>().AddForceAtPosition (100.0f* fwrd ,contact.point);
		hit.transform.SendMessageUpwards ("Damage",damage, SendMessageOptions.DontRequireReceiver);
		Destroy(GetComponent<Rigidbody>());



	}
}
