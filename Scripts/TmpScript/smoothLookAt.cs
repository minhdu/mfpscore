using UnityEngine;
using System.Collections;

public class smoothLookAt : MonoBehaviour {
	public Transform target;
	public float damping = 6.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
		Quaternion rotation = Quaternion.LookRotation(target.position - transform.position);
		transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
	
	}
}
