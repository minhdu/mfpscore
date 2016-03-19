using UnityEngine;
using System.Collections;

public class playerrotate : MonoBehaviour {
	private float sensitivityX = 6.0f;
	public float aimSens = 2.0f;
	public float normalSens= 6f;
	private float rotationX = 0f;
	public float speed = 1.0f;
	public float smooth = 0.5f;

	void Start () {
	
	}
	

	void Update () {

		if (Input.GetButton("Aim"))
		{
			sensitivityX = aimSens;
		}
		else
		{
			sensitivityX = normalSens;
		}

		rotationX = Input.GetAxis ("Mouse X") * sensitivityX * smooth * (Time.deltaTime * speed);
		

		
		

		
		
		transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y + rotationX, 0);
	
	}
}
