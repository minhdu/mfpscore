using UnityEngine;
using System.Collections;

public class CameraRotate : MonoBehaviour {
	
	float sensitivityX = 6.0f;
	float sensitivityY = 6.0f;
	public float aimSens = 2.0f;
	public float normalSens= 6f;
	float rotationX = 0f;
	float rotationY = 0f;
	public float speed = 1.0f;
	public float smooth = 0.5f;

	Transform trans;

	void Start () {
		trans = GetComponent<Transform> ();
	}
	
	void Update () {

#if UNITY_EDITOR
		//Cursor.lockState = CursorLockMode.Locked;
#endif

		if (WeaponHandler.Instance.CurrentWeapon.IsAiming()) {
			sensitivityX = aimSens;
		}
		else {
			sensitivityX = normalSens;
		}

#if UNITY_EDITOR
		rotationX = Input.GetAxis ("Mouse X") * sensitivityX * smooth * (Time.deltaTime * speed);
		rotationY = Input.GetAxis ("Mouse Y") * sensitivityY * smooth * (Time.deltaTime * speed);
#else
		rotationX = FPSCamera.Instance.XInput * sensitivityX * smooth * Time.deltaTime * speed;
		rotationY = FPSCamera.Instance.YInput * sensitivityY * smooth * Time.deltaTime * speed;
#endif

		trans.localEulerAngles = new Vector3(trans.localEulerAngles.x - rotationY, trans.localEulerAngles.y + rotationX, 0);
	}
}
