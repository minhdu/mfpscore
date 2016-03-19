using UnityEngine;

public class WeaponSway : MonoBehaviour {

	public float amount = 0.02f;
	public float maxAmount = 0.03f;
	public float Smooth = 3;
	public float SmoothRotation = 2;
	public int tiltAngle = 25;

	public bool deadzone = false;

	float rotX = 0;
	float rotY = 0;

	public	float sensitivity = 3;

	public float minRotX = -10;
	public float maxRotX = 10;
	public float minRotY = -10;
	public float maxRotY = 10;

	public float offsetY = 0;
	public float offsetX = 0;

	Transform trans;

	Vector3 def;

	void Start () {
		trans = GetComponent<Transform> ();
		def = trans.localPosition;
	}

	void Update () {
		if(deadzone == true) {
			#if UNITY_EDITOR
			rotX += (-Input.GetAxis("Mouse Y") * sensitivity - offsetY);
			rotY += (Input.GetAxis("Mouse X") * sensitivity + offsetX);      
			#else
			rotX += (-FPSCamera.Instance.YInput * sensitivity - offsetY);
			rotY += (FPSCamera.Instance.XInput * sensitivity + offsetX); 
			#endif

			var target2 = Quaternion.Euler(rotX, rotY, 0);

			if(rotX > maxRotX)
				rotX = maxRotX;

			if(rotX < minRotX)
				rotX = minRotX;

			if(rotY > maxRotY)
				rotY = maxRotY;

			if(rotY < minRotY)
				rotY = minRotY;
			trans.localRotation = target2;
		}

		offsetX = 0;
		offsetY = 0;

		if(deadzone == false) {
#if UNITY_EDITOR
			float factorX = -Input.GetAxis("Mouse X") * amount;
			float factorY = -Input.GetAxis("Mouse Y") * amount;
#else
			float factorX = -FPSCamera.Instance.XInput * amount;
			float factorY = -FPSCamera.Instance.YInput * amount;
#endif
			if(factorX > maxAmount)
				factorX = maxAmount;

			if(factorX < -maxAmount)
				factorX = -maxAmount;

			if(factorY > maxAmount)
				factorY = maxAmount;

			if(factorY < -maxAmount)
				factorY = -maxAmount;


			Vector3 Final = new Vector3(def.x+factorX, def.y+factorY, def.z);
			trans.localPosition = Vector3.Slerp(trans.localPosition, Final, Time.deltaTime * Smooth);

#if UNITY_EDITOR
			float tiltAroundZ = Input.GetAxis("Mouse X") * tiltAngle;
			float tiltAroundX = Input.GetAxis("Mouse Y") * tiltAngle;
#else
			float tiltAroundZ = FPSCamera.Instance.XInput * tiltAngle;
			float tiltAroundX = FPSCamera.Instance.YInput * tiltAngle;
#endif
			Quaternion target = Quaternion.Euler (tiltAroundX, 0, tiltAroundZ);
			trans.localRotation = Quaternion.Slerp(trans.localRotation, target,Time.deltaTime * SmoothRotation);
		}
	}
}