using UnityEngine;
using System.Collections;

public class camerarotate : MonoBehaviour {

	private float  sensitivityY  = 6f;
	public float minimumY = -70f;
	public float maximumY  = 70f;
	private float rotationY = 0f;
	public float aimSens= 2f;
	public float normalSens= 6f;
	public float speed = 1.0f;
	public float smooth = 0.5f;

	void Start () {
	
	}
	

	void Update () {

		//Cursor.lockState = CursorLockMode.Locked;

		if (Input.GetButton("Aim"))

		{
sensitivityY = aimSens;
		}
		else
		{
sensitivityY = normalSens;
		}

		rotationY += Input.GetAxis("Mouse Y") * sensitivityY * smooth * (Time.deltaTime * speed);
		
		
		
		rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
		
		
		transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
	
	}
	void dorecoil(float recoil)
	{
			

		rotationY += recoil * Time.deltaTime * 20f;

	}
}
