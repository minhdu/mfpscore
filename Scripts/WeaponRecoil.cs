using UnityEngine;
using System.Collections;

public class WeaponRecoil : Singleton<WeaponRecoil> {
	float muzzleClimbAngle = 2.0f;

	float maxClimbX = 1;
	float minClimbX = -1;

	WeaponSway weaponSway;

	void Start() {
		weaponSway = GetComponent<WeaponSway>();
	}

	public void Aim() {
		weaponSway.deadzone = false;
	}

	public void EndAim() {
		weaponSway.deadzone = true;
	}

	public void IsFiring()
	{
		weaponSway.offsetY = muzzleClimbAngle;
		weaponSway.offsetY = Random.Range(minClimbX, maxClimbX);
	}
}
