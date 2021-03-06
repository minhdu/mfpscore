﻿using UnityEngine;
using System.Collections;

public class FPSCamera : Singleton<FPSCamera> {

	TouchAreaData cameraHandleArea = new TouchAreaData();

	float xInput;
	public float XInput {
		get {
			return xInput;
		}
	}

	float yInput;
	public float YInput {
		get {
			return yInput;
		}
	}


	Vector2 slideInput;

	float xSensitive = 2000f;
	float ySensitive = 900F;

	public Transform Transform {
		get {
			return cameraTrans;
		}
	}

	public Transform playerTrans;

	public float zSmooth = 10;

	public float rotateSensitive = 1.5f;

	public Vector2 shakeAmount;
	public Vector3 originalPosition;

	public Transform weaponRoot;

	public bool isInited = false;

	Transform cameraTrans;
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
		cameraTrans = PlayerCamera.GetComponent<Transform> ();
		originalPosition = cameraTrans.localPosition;
		isInited = true;
	}

	void Update () {
		if (isInited) {
			HandleTouch ();
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
				CrossHair.Instance.Zoom ();
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
					CrossHair.Instance.Reverts ();
					slideInput =  Vector2.zero;
					cameraHandleArea.FingerDown = false;
					cameraHandleArea.FingerId = -1;
				}
			}
		}

		xInput = (slideInput.x  * ((xSensitive*rotateSensitive) * 0.01f)) / ScreenHelper.DPI;
		yInput = (slideInput.y * ((ySensitive*rotateSensitive) * 0.01f)) / ScreenHelper.DPI;
	}
}
