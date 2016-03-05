using UnityEngine;
using System.Collections;

public class FPSCamera : Singleton<FPSCamera> {

	TouchAreaData cameraHandleArea = new TouchAreaData();

	Vector2 slideInput;
	float originalRotation;
	float xInput;
	float yInput;
	float xSensitive = 2000f;
	float ySensitive = 900F;
	float yRotation = 0.0f;
	float xRotation = 0.0f;
	float rotateCoef = 1f;
	float minYAngle = -60F;
	float maxYAngle = 60F;

	Transform mTransform;
	PlayerController playerController;

	public float zSmooth = 10;
	public float xPosition = 0;
	public float yPosition = 0;
	public float zPosition = 0;
	public float yCoef = 0;

	public float rotateSensitive = 1.5f;
	public Transform weaponRoot;

	public bool isInited = false;

	Transform playerCamTrans;
	Camera playerCamera;
	public Camera PlayerCamera {
		get {
			if (playerCamera == null)
				playerCamera = GetComponent<Camera> ();

			return playerCamera;
		}
	}

	void Start () {
		InitCamera ();
	}

	void InitCamera () {
		cameraHandleArea.FingerBound = new Rect(0,0, ScreenHelper.ScreenSize.x*3, ScreenHelper.ScreenSize.y);
		mTransform = GetComponent<Transform> ();
		playerController = GetComponentInParent<PlayerController> ();
		playerCamTrans = PlayerCamera.GetComponent<Transform> ();
		originalRotation = mTransform.rotation.eulerAngles.y;
		isInited = true;
	}

	void Update () {
		if (isInited) {
			HandleTouch ();
			RotatePlayer ();
			HandleWeapon ();
		}
	}

	void HandleTouch () {
		foreach  (Touch touch in Input.touches)
		{
			// Began Touch Phase
			if (touch.phase == TouchPhase.Began) 
			{
				// Camera Control
				if(!cameraHandleArea.FingerDown){
					cameraHandleArea.FingerDown = cameraHandleArea.FingerBound.Contains(touch.position);
					if(cameraHandleArea.FingerDown){
						cameraHandleArea.FingerId = touch.fingerId;
					}
				}
			}
			// Move Phase Begin
			else if (touch.phase == TouchPhase.Moved)
			{

				if (cameraHandleArea.FingerDown && cameraHandleArea.FingerId == touch.fingerId){
					slideInput = touch.deltaPosition * Time.smoothDeltaTime;
				}
			}
			// Stationary Phase Begin
			else if (touch.phase == TouchPhase.Stationary)
			{
				if (cameraHandleArea.FingerDown && cameraHandleArea.FingerId == touch.fingerId){
					slideInput = Vector2.zero;
				}
			}
			// End or Canceled Phase Begin
			else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
			{		
				if (cameraHandleArea.FingerDown && cameraHandleArea.FingerId == touch.fingerId){
					slideInput =  Vector2.zero;
					cameraHandleArea.FingerDown = false;
					cameraHandleArea.FingerId = -1;
				}
			}
		}
	}


	void RotatePlayer () {
		xInput = (slideInput.x  * ((xSensitive*rotateSensitive) * 0.01f)) / ScreenHelper.DPI;
		yInput = (slideInput.y * ((ySensitive*rotateSensitive) * 0.01f)) / ScreenHelper.DPI;

		xRotation += xInput * rotateCoef;
		yRotation += yInput * rotateCoef;
		yRotation = Mathf.Clamp(yRotation, minYAngle, maxYAngle);

		playerController.Transform.rotation =  Quaternion.Slerp (playerController.Transform.rotation, Quaternion.Euler(0, originalRotation + xRotation, 0),  0.1f);
		playerCamTrans.localRotation =  Quaternion.Slerp (playerCamTrans.localRotation, Quaternion.Euler(playerCamTrans.localRotation.x - yRotation, 0, 0),  0.1f);
	}

	void HandleWeapon () {
		
		Weapon currentWeapon = PlayerController.Instance.CurrentWeapon;

		if (xInput < -1.5f && weaponRoot.localPosition.x > -0.02f) {
			xPosition = -0.02f;
		} else if (xInput > 1.5f && weaponRoot.localPosition.x < 0.02f) {
			xPosition = 0.02f;
		}

		if (yInput < -1f && weaponRoot.localPosition.y > -0.009f) {
			yCoef = 0.009f;
		} else if (yInput > 1f && weaponRoot.localPosition.y < 0.009f) {
			yCoef = -0.009f;
		}

		if(!PlayerController.Instance.IsAiming){
			if(playerCamera.fieldOfView < 42){
				playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, 42, 0.1f);
			}

			// Apply slide effect on arms
			weaponRoot.localPosition = Vector3.Lerp(weaponRoot.localPosition, new Vector3(xPosition,yPosition+yCoef,zPosition),  Time.deltaTime * zSmooth);

			// Reinit the rotate speed
			weaponRoot.localRotation =  Quaternion.Slerp (weaponRoot.localRotation, Quaternion.Euler(playerController.InClin, 0, 0),  0.1f);
			rotateCoef = 1f;

//			if (MSPControl.HideCrossHair && CurrentWeapon.firearms){
//				MSPControl.HideCrossHair = !MSPControl.HideCrossHair;
//			}
		}else if (PlayerController.Instance.IsAiming && !PlayerController.Instance.IsReloading && PlayerController.Instance.CurrentWeapon.isFirearms){
			if(playerCamera.fieldOfView > 20){
				playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, 20, 0.1f);
			}

			weaponRoot.localPosition = Vector3.Lerp(weaponRoot.localPosition, new Vector3(currentWeapon.aimPosition.x,
				yPosition + currentWeapon.aimPosition.y, zPosition / 2 + currentWeapon.aimPosition.z), Time.deltaTime * zSmooth);
			
			weaponRoot.localRotation =  Quaternion.Slerp (weaponRoot.localRotation, Quaternion.Euler(currentWeapon.aimAngle, 0, 0),  0.1f);

			// Divide the rotate speed by 2
			rotateCoef = 0.5f;

//			if (!MSPControl.HideCrossHair){
//				MSPControl.HideCrossHair = !MSPControl.HideCrossHair;
//			}
		}
	}
}
